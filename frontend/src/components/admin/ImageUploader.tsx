import { useState, useRef } from 'react';
import { mediaApi } from '../../api/media';

export default function ImageUploader({ value, onChange, onUploadingChange }: { value?: string; onChange: (url: string) => void; onUploadingChange?: (uploading: boolean) => void }) {
  const [uploading, setUploading] = useState(false);
  const [progress, setProgress] = useState(0);
  const [error, setError] = useState('');
  const inputRef = useRef<HTMLInputElement>(null);

  async function handleFile(file: File) {
    setError('');
    if (!['image/jpeg', 'image/png', 'image/webp'].includes(file.type) || file.size > 8 * 1024 * 1024 || file.size === 0) {
      setError('Choose a JPG, PNG or WEBP image up to 8 MB.');
      return;
    }
    onUploadingChange?.(true);
    setUploading(true);
    try {
      const media = await mediaApi.upload(file, setProgress);
      onChange(media.blobUrl);
    } catch {
      setError('Upload failed. Check file type (JPG/PNG/WEBP) and size (max 8MB).');
    } finally {
      setUploading(false);
      onUploadingChange?.(false);
      if (inputRef.current) inputRef.current.value = '';
      setProgress(0);
    }
  }

  return (
    <div>
      {value && <img src={value} alt="Preview" style={{ width: '100%', maxHeight: 200, objectFit: 'cover', borderRadius: 6, marginBottom: 8 }} />}
      <input
        ref={inputRef}
        type="file"
        aria-label="Upload image from your computer"
        disabled={uploading}
        accept="image/jpeg,image/jpg,image/png,image/webp"
        onChange={(e) => e.target.files?.[0] && handleFile(e.target.files[0])}
      />
      <p style={{ fontSize: 12 }}>Choose an image from your computer (JPG, PNG or WEBP, up to 8 MB).</p>
      {value && <button type="button" disabled={uploading} onClick={() => onChange('')}>Remove image from article</button>}
      {uploading && <p style={{ fontSize: 12 }}>Uploading… {progress}%</p>}
      {error && <p className="error-state" style={{ padding: 0, fontSize: 12 }}>{error}</p>}
    </div>
  );
}
