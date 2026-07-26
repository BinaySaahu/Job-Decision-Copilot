from pydantic import BaseModel, Field
from typing import List, Optional

class ResumeTextRequest(BaseModel):
    resume_text: str = Field(..., min_length=1)

class ParsedResumeResponse(BaseModel):
    skills: List[str]
    roles: List[str]
    experience: Optional[int] = None
    education: Optional[List[str]] = None
    certifications: Optional[List[str]] = None
    warnings: Optional[List[str]] = None
