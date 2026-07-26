# Resume Parsing FastAPI Service

This service accepts raw resume text and returns normalized structured resume data.

## Install

```bash
cd fastapi
conda create -n myenv python=3.12
condat activate myenv
pip install -r requirements.txt
```

## Run

```bash
uvicorn app:app --reload
```

## Environment

- `GROK_API_KEY` - API key for Groq

You can also create a `.env` file in `fastapi/` with:

```env
GROK_API_KEY=your-Groq-api-key
```
