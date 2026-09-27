import { useMemo, useRef, useState } from 'react';
import ReactQuill from 'react-quill-new';
import { mediaApi } from '../../api/media';
import 'react-quill-new/dist/quill.snow.css';

export default function RichTextEditor({ value, onChange, onUploadingChange }: { value: string; onChange: (html: string) => void; onUploadingChange?: (busy: boolean) => void }) {
  const editor = useRef<ReactQuill>(null);
  const picker = useRef<HTMLInputElement>(null);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');
  const modules = useMemo(() => ({ toolbar: {
    container: [[{ header: [1, 2, 3, false] }], ['bold', 'italic', 'underline', 'blockquote'], [{ list: 'ordered' }, { list: 'bullet' }], ['link', 'image'], ['clean']],
    handlers: { image: () => picker.current?.click() },
  }}), []);
  async function upload(file: File) {
    setError('');
    if (!['image/jpeg', 'image/png', 'image/webp'].includes(file.type) || !file.size || file.size > 8 * 1024 * 1024) {
      setError('Choose a JPG, PNG or WEBP image up to 8 MB.'); return;
    }
    setBusy(true); onUploadingChange?.(true);
    try {
      const q = editor.current?.getEditor();
      const position = q?.getSelection()?.index ?? Math.max(0, (q?.getLength() ?? 1) - 1);
      const media = await mediaApi.upload(file);
      q?.insertEmbed(position, 'image', media.blobUrl, 'user');
      q?.setSelection(position + 1, 0);
    } catch { setError('Image upload failed. Please try again.'); }
    finally { setBusy(false); onUploadingChange?.(false); if (picker.current) picker.current.value = ''; }
  }
  return <div>
    <input ref={picker} type="file" hidden disabled={busy} accept="image/jpeg,image/png,image/webp" onChange={e => { if (e.target.files?.[0]) void upload(e.target.files[0]); }} />
    <ReactQuill ref={editor} theme="snow" value={value} onChange={onChange} modules={modules} readOnly={busy} />
    {busy && <p role="status">Uploading image…</p>}
    {error && <p role="alert">{error}</p>}
  </div>;
}
