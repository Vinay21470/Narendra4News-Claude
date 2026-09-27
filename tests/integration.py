"""Disposable SQL Server + Azurite regression flow; requires N4NC_TEST_SQL."""
import base64, json, os, pathlib, secrets, subprocess, tempfile, time, urllib.request, urllib.error
ROOT = pathlib.Path(__file__).resolve().parents[1]
BASE = 'http://127.0.0.1:5189'
PNG = base64.b64decode('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+jRZkAAAAASUVORK5CYII=')
TOKEN = ''
def request(path, method='GET', body=None, auth=False, content_type='application/json', expected=200):
    if isinstance(body, dict): body = json.dumps(body).encode()
    headers = {'Content-Type': content_type}
    if auth: headers['Authorization'] = 'Bearer ' + TOKEN
    req = urllib.request.Request(BASE + path, data=body, method=method, headers=headers)
    try:
        with urllib.request.urlopen(req, timeout=10) as res:
            code, raw, mime = res.status, res.read(), res.headers.get('Content-Type', '')
    except urllib.error.HTTPError as e:
        code, raw, mime = e.code, e.read(), e.headers.get('Content-Type', '')
    assert code == expected, (path, code, raw[:500])
    return json.loads(raw) if 'json' in mime else raw

def upload(data, auth=True, expected=200):
    boundary = 'n4nc-test-boundary'
    body = (f'--{boundary}\r\nContent-Disposition: form-data; name="file"; filename="test.png"\r\nContent-Type: image/png\r\n\r\n'.encode()
            + data + f'\r\n--{boundary}--\r\n'.encode())
    return request('/api/media/upload', 'POST', body, auth, 'multipart/form-data; boundary=' + boundary, expected)

with tempfile.TemporaryDirectory() as tmp:
    password = 'A1!' + secrets.token_hex(20)
    env = dict(os.environ, ASPNETCORE_ENVIRONMENT='Production', ASPNETCORE_URLS=BASE,
               AZURE_SQL_CONNECTION_STRING=os.environ['N4NC_TEST_SQL'], AZURE_STORAGE_CONNECTION_STRING='UseDevelopmentStorage=true',
               AZURE_STORAGE_CONTAINER='testmedia', PUBLIC_API_URL=BASE, JWT_SECRET=secrets.token_hex(32),
               ADMIN_EMAIL='admin@example.test', ADMIN_PASSWORD=password)
    log = open(pathlib.Path(tmp) / 'api.log', 'w+')
    bloblog = open(pathlib.Path(tmp) / 'blob.log', 'w+')
    blob = subprocess.Popen(['azurite', '--silent', '--location', tmp], stdout=bloblog, stderr=subprocess.STDOUT)
    api = None
    try:
        time.sleep(2)
        api = subprocess.Popen(['dotnet', str(ROOT / 'backend/Narendra4News.API/bin/Release/net9.0/Narendra4News.API.dll')],
                               cwd=ROOT / 'backend/Narendra4News.API', env=env, stdout=log, stderr=subprocess.STDOUT)
        for i in range(180):
            try:
                request('/health'); break
            except Exception:
                if api.poll() is not None: raise RuntimeError('API exited before health check')
                time.sleep(1)
        else: raise RuntimeError('API startup timed out')
        TOKEN = request('/api/auth/login', 'POST', {'email':env['ADMIN_EMAIL'], 'password':password})['data']['token']
        upload(PNG, auth=False, expected=401)
        upload(b'not an image', expected=400)
        media = upload(PNG)['data']
        assert request(media['blobUrl'].removeprefix(BASE)) == PNG
        # The underlying blob must not be anonymously accessible.
        try:
            urllib.request.urlopen('http://127.0.0.1:10000/devstoreaccount1/testmedia/' + media['fileName'])
            raise AssertionError('Blob container is public')
        except urllib.error.HTTPError as e: assert e.code in (403,404,409)
        category = request('/api/categories')['data'][0]['id']
        draft = dict(title='Upload regression', slug='upload-' + secrets.token_hex(4), shortDescription='Test', content='<p>Test</p>',
                     featuredImageUrl=media['blobUrl'], categoryId=category, status='Draft', seoKeywords='preserved')
        article = request('/api/articles', 'POST', draft, True, expected=201)['data']
        aid = article['id']
        request('/api/articles/' + article['slug'], expected=404)
        request(f'/api/articles/admin/{aid}', expected=401)
        saved = request(f'/api/articles/admin/{aid}', auth=True)['data']
        assert saved['categoryId'] == category and saved['status'] == 'Draft' and saved['featuredImageUrl'] == media['blobUrl']
        draft['status'] = 'Published'
        request(f'/api/articles/{aid}', 'PUT', draft, True)
        public = request('/api/articles/' + article['slug'])['data']
        assert public['seoKeywords'] == 'preserved' and public['featuredImageUrl'] == media['blobUrl']
        assert request(public['featuredImageUrl'].removeprefix(BASE)) == PNG
        print('PASS: SQL migrations, admin login, upload validation, private blob image delivery, draft privacy, edit and publish.')
    except Exception:
        log.flush(); log.seek(0)
        print(log.read()[-16000:].replace(password, '[redacted]'))
        raise
    finally:
        for proc in (api, blob):
            if proc and proc.poll() is None:
                proc.terminate()
                try: proc.wait(timeout=10)
                except subprocess.TimeoutExpired: proc.kill()
        log.close(); bloblog.close()
