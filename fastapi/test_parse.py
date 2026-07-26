import json
import urllib.request

url = 'http://127.0.0.1:8000/parse-resume'
payload = json.dumps({
    'resume_text': 'John Doe\nSoftware Engineer\nSkills\nJava\nSpring Boot\nSQL\nDocker\nExperience\n3 years\nEducation\nBSc Computer Science\nCertifications\nAWS Certified Developer'
}).encode('utf-8')
req = urllib.request.Request(url, data=payload, headers={'Content-Type': 'application/json'})
with urllib.request.urlopen(req) as resp:
    print(resp.status)
    print(resp.read().decode('utf-8'))
