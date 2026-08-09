import os
import sys
import json
import tarfile
import urllib.request
import urllib.parse
from google.oauth2 import service_account
import google.auth.transport.requests

sys.stdout.reconfigure(encoding='utf-8')
script_dir = os.path.dirname(os.path.abspath(__file__))
creds_path = os.path.join(os.path.dirname(script_dir), "audio_book", "credentials_b.json")

if not os.path.exists(creds_path):
    print(f"❌ 找不到憑證檔: {creds_path}")
    sys.exit(1)

creds = service_account.Credentials.from_service_account_file(
    creds_path, scopes=['https://www.googleapis.com/auth/cloud-platform']
)
auth_req = google.auth.transport.requests.Request()
creds.refresh(auth_req)
token = creds.token

bucket_name = "audiobook-project-b5c7713d-0d34-467f-a53"
obj_name = "llplayer-builds/LLPlayer-win-x64.tar.gz"
encoded_name = urllib.parse.quote(obj_name, safe='')
media_url = f"https://storage.googleapis.com/download/storage/v1/b/{bucket_name}/o/{encoded_name}?alt=media"

req = urllib.request.Request(media_url)
req.add_header('Authorization', f'Bearer {token}')

tar_save_path = os.path.join(script_dir, "LLPlayer-win-x64.tar.gz")
print(f"⬇️ 下載 Google Cloud Build 編譯成果: {obj_name} ...", end="", flush=True)

with urllib.request.urlopen(req) as resp, open(tar_save_path, 'wb') as f:
    f.write(resp.read())

print(f" ✅ 下載完成 ({os.path.getsize(tar_save_path) / (1024*1024):.2f} MB)")

output_dir = os.path.join(script_dir, "output_build")
os.makedirs(output_dir, exist_ok=True)

print(f"📦 解壓縮編譯檔至: {output_dir} ...", end="", flush=True)
with tarfile.open(tar_save_path, 'r:gz') as tar:
    tar.extractall(path=output_dir)

print(" ✅ 解壓縮完成！")
print("=" * 65)
print(f"🎉 Google Cloud 編譯成品已就緒：{output_dir}")
print("=" * 65)
