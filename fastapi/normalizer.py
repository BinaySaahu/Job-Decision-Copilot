import json
import os
import re
from typing import List, Optional
from rapidfuzz import process, fuzz
from openai import OpenAI
from models import ParsedResumeResponse
from dotenv import load_dotenv
from groq import Groq

load_dotenv()

CANONICAL_SKILLS = {
    "spring": "Spring Boot",
    "spring boot": "Spring Boot",
    "java": "Java",
    "sql": "SQL",
    "docker": "Docker",
    "aws": "AWS",
    "c#": "C#",
    "dotnet": ".NET",
    "javascript": "JavaScript",
    "typescript": "TypeScript"
}

CANONICAL_ROLES = {
    "backend developer": "Backend Developer",
    "software engineer": "Software Engineer",
    "full stack developer": "Full Stack Developer",
    "devops engineer": "DevOps Engineer",
    "data engineer": "Data Engineer"
}

CANONICAL_CERTIFICATIONS = {
    "aws certified developer": "AWS Certified Developer",
    "aws certified solutions architect": "AWS Certified Solutions Architect"
}

GROK_API_KEY = os.getenv("GROK_API_KEY")
client = Groq(
    api_key=GROK_API_KEY
) if GROK_API_KEY else None


def normalize_term(term: str, canonical_map: dict, threshold: int = 80) -> str:
    cleaned = term.strip().lower()
    if cleaned in canonical_map:
        return canonical_map[cleaned]

    best = process.extractOne(cleaned, list(canonical_map.keys()), scorer=fuzz.token_sort_ratio)
    if best and best[1] >= threshold:
        return canonical_map[best[0]]

    return term.strip()


async def normalize_with_ai(term: str, category: str) -> str:
    if client is None:
        return term.strip()

    known_values = CANONICAL_SKILLS if category == "skill" else CANONICAL_ROLES
    prompt = (
        f"Normalize the following {category} to one of the known canonical values if possible. "
        f"Use exact canonical values when the input is a close match. "
        f"If no match is appropriate, return the original text exactly as provided.\n\n"
        f"Known canonical values: {', '.join(sorted(known_values.values()))}\n"
        f"Input: {term}\n"
        f"Normalized:"
    )

    response = client.chat.completions.create(
        model="llama-3.1-8b-instant",
        messages=[{"role": "user", "content": prompt}],
        max_tokens=40,
        temperature=0.0
    )

    normalized = response.choices[0].message.content.strip()
    return normalized or term.strip()


class ResumeNormalizer:
    def __init__(self):
        self.skills_map = CANONICAL_SKILLS
        self.roles_map = CANONICAL_ROLES
        self.certifications_map = CANONICAL_CERTIFICATIONS

    async def parse_and_normalize(self, resume_text: str) -> ParsedResumeResponse:
        if not resume_text or not resume_text.strip():
            raise ValueError("Resume text must not be empty.")

        parsed = await self.extract_structured_resume(resume_text)

        # warnings = []
        # skills = await self.normalize_list(parsed.skills, self.skills_map, "skill", warnings)
        # roles = await self.normalize_list(parsed.roles, self.roles_map, "role", warnings)
        # education = await self.normalize_list(parsed.education, {}, "education", warnings)
        # certifications = await self.normalize_list(parsed.certifications, self.certifications_map, "certification", warnings)

        skills = parsed.skills
        roles  = parsed.roles
        education = parsed.education
        certifications = parsed.certifications

        if parsed.experience is None:
            raise ValueError("Parsed resume does not contain experience years.")

        res = ParsedResumeResponse(
            skills=skills,
            roles=roles,
            experience=parsed.experience,
            education=education,
            certifications=certifications,
            warnings= None
        )
        

        return res

    async def extract_structured_resume(self, text: str) -> ParsedResumeResponse:
        if client is None:
            return self.fallback_extract(text)

        prompt = self.build_prompt(text)
        response = client.chat.completions.create(
            model="llama-3.1-8b-instant",
            messages=[{"role": "user", "content": prompt}],
            max_tokens=400,
            temperature=0.0
        )

        raw = response.choices[0].message.content.strip()
        print("LLM response: " + repr(raw))
        try:
            data = json.loads(raw)
            # print(data)
        except json.JSONDecodeError:
            raise ValueError("AI response could not be parsed as JSON.")

        return ParsedResumeResponse(**data)

    def build_prompt(self, text: str) -> str:
        return f"""
    You are an expert resume parser.
    Your task is to extract structured information from the resume below.
    IMPORTANT INSTRUCTIONS:
    - Return ONLY a valid JSON object.
    - Do NOT include markdown (no ``` or ```json).
    - Do NOT include explanations, notes, or comments.
    - Do NOT wrap the JSON in quotes.
    - The output must be directly parseable using Python's json.loads().
    - Do NOT invent information that is not present in the resume.
    - Every field below MUST be present.
    - If a field is unavailable:
    - Use [] for arrays.
    - Use 0 for experience.
    - Use "" for missing strings.
    - Use null for missing numeric values such as CGPA.
    Normalize skills and roles to the closest matching canonical value whenever possible.
    If no close match exists, preserve the original value.
    Canonical Skills:{', '.join(sorted(self.skills_map.values()))}
    Canonical Roles:{', '.join(sorted(self.roles_map.values()))}
    Return JSON in EXACTLY this format:{{
    "skills": ["string"],
    "roles": ["string"],
    "experience": 0,
    "education": ["string"],
    "certifications": ["string"],
    "warnings": ["string"]
    }}
    Resume:
    {text}
    Return ONLY the JSON object.
    """

    def fallback_extract(self, text: str) -> ParsedResumeResponse:
        lines = [line.strip() for line in text.splitlines() if line.strip()]
        skills = self.extract_section(lines, "skills")
        roles = self.extract_section(lines, "roles")
        experience = self.extract_experience(lines)
        education = self.extract_section(lines, "education")
        certifications = self.extract_section(lines, "certifications")

        return ParsedResumeResponse(
            skills=skills,
            roles=roles,
            experience=experience,
            education=education,
            certifications=certifications,
            warnings=[]
        )

    async def normalize_list(self, values: Optional[List[str]], canonical_map: dict, category: str, warnings: List[str]) -> List[str]:
        if not values:
            return []

        result = []
        for value in values:
            normalized = normalize_term(value, canonical_map)
            if normalized == value.strip() and client is not None and value.strip().lower() not in canonical_map:
                normalized = await normalize_with_ai(value, category)
            if normalized.lower() != value.strip().lower():
                if len(value.strip()) > 0:
                    warnings.append(f"Normalized {category}: '{value}' -> '{normalized}'")
            if normalized not in result:
                result.append(normalized)

        return result

    def extract_section(self, lines: List[str], header: str) -> List[str]:
        header = header.lower()
        output = []
        collecting = False
        for line in lines:
            lower = line.lower()
            if collecting and lower in {"skills", "roles", "experience", "education", "certifications"}:
                break
            if collecting and line:
                output.append(line)
            if lower == header:
                collecting = True
        return output

    def extract_experience(self, lines: List[str]) -> Optional[int]:
        for line in lines:
            match = re.search(r"(\d+)\+?\s*(years|yrs|year)", line, re.IGNORECASE)
            if match:
                return int(match.group(1))
        return None
