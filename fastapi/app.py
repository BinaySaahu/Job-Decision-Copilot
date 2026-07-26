from fastapi import FastAPI, HTTPException
from pydantic import BaseModel, Field, ValidationError
from typing import List, Optional
from enum import Enum
from models import ParsedResumeResponse, ResumeTextRequest
from normalizer import ResumeNormalizer

app = FastAPI(title="Resume Parsing API")

normalizer = ResumeNormalizer()

@app.post("/parse-resume", response_model=ParsedResumeResponse)
async def parse_resume(request: ResumeTextRequest):
    try:
        parsed = await normalizer.parse_and_normalize(request.resume_text)
        return parsed
    except ValueError as exc:
        raise HTTPException(status_code=400, detail=str(exc))
    except Exception as exc:
        print(str(exc))
        raise HTTPException(status_code=500, detail="Failed to parse resume text.")
