import { useEffect, useState, type CSSProperties } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { articlesApi } from '../../api/articles';
import { categoriesApi } from '../../api/categories';
import { moviesApi } from '../../api/movies';
import RichTextEditor from '../../components/admin/RichTextEditor';
import ImageUploader from '../../components/admin/ImageUploader';
import { slugify } from '../../utils/format';
import Toast from '../../components/common/Toast';

interface FormState {
  title: string;
  slug: string;
  shortDescription: string;
  content: string;
  featuredImageUrl: string;
  categoryId: number | '';
  movieId: number | '';
  status: 'Draft' | 'Published' | 'Archived' | 'Scheduled';
  publishedDate: string;
  isFeatured: boolean;
  isTrending: boolean;
  seoTitle: string;
  seoDescription: string;
  seoKeywords: string;
}

const EMPTY: FormState = {
  title: '', slug: '', shortDescription: '', content: '', featuredImageUrl: '',
  categoryId: '', movieId: '', status: 'Draft', publishedDate: '',
  isFeatured: false, isTrending: false, seoTitle: '', seoDescription: '', seoKeywords: '',
};

export default function ArticleEditor() {
  const { id } = useParams();
  const isEdit = !!id;
  const navigate = useNavigate();
  const [form, setForm] = useState<FormState>(EMPTY);
  const [saving, setSaving] = useState(false);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);

  const { data: categories } = useQuery({ queryKey: ['categories-all'], queryFn: () => categoriesApi.getAll(false) });
  const { data: movies } = useQuery({ queryKey: ['movies-for-select'], queryFn: () => moviesApi.getAll(1, 100) });

  const [uploading, setUploading] = useState(false);
  const [inlineUploading, setInlineUploading] = useState(false);
  const { data: existing, isLoading: loadingArticle, isError: loadError } = useQuery({
    queryKey: ['admin-article', id], queryFn: () => articlesApi.getForAdmin(Number(id)), enabled: isEdit,
  });
  useEffect(() => {
    if (!existing) return;
    const d = existing.publishedDate ? new Date(existing.publishedDate) : null;
    const localDate = d ? new Date(d.getTime() - d.getTimezoneOffset() * 60000).toISOString().slice(0, 16) : '';
    setForm({
      title: existing.title, slug: existing.slug, shortDescription: existing.shortDescription, content: existing.content,
      featuredImageUrl: existing.featuredImageUrl ?? '', categoryId: existing.categoryId, movieId: existing.movieId ?? '',
      status: existing.status, publishedDate: localDate, isFeatured: existing.isFeatured, isTrending: existing.isTrending,
      seoTitle: existing.seoTitle ?? '', seoDescription: existing.seoDescription ?? '', seoKeywords: existing.seoKeywords ?? '',
    });
  }, [existing]);

  function update<K extends keyof FormState>(key: K, value: FormState[K]) {
    setForm((f) => ({ ...f, [key]: value }));
  }

  async function handleSave(status: FormState['status']) {
    if (uploading || inlineUploading) return;
    if (!form.title || !form.categoryId) {
      setToast({ message: 'Title and Category are required.', type: 'error' });
      return;
    }
    setSaving(true);
    const payload = {
      title: form.title,
      slug: form.slug || slugify(form.title),
      shortDescription: form.shortDescription,
      content: form.content,
      featuredImageUrl: form.featuredImageUrl || undefined,
      categoryId: Number(form.categoryId),
      movieId: form.movieId ? Number(form.movieId) : undefined,
      status,
      publishedDate: form.publishedDate ? new Date(form.publishedDate).toISOString() : undefined,
      isFeatured: form.isFeatured,
      isTrending: form.isTrending,
      seoTitle: form.seoTitle || undefined,
      seoDescription: form.seoDescription || undefined,
      seoKeywords: form.seoKeywords || undefined,
    };
    try {
      if (isEdit) {
        await articlesApi.update(Number(id), payload);
      } else {
        await articlesApi.create(payload);
      }
      setToast({ message: `Article ${status === 'Published' ? 'published' : 'saved'} successfully.`, type: 'success' });
      setTimeout(() => navigate('/admin/articles'), 800);
    } catch {
      setToast({ message: 'Failed to save article.', type: 'error' });
    } finally {
      setSaving(false);
    }
  }

  return (
    <div>
      <h1>{isEdit ? 'Edit Article' : 'New Article'}</h1>
      {loadError && <p role="alert">Could not load this article. Reload before editing.</p>}
      <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr', gap: 24 }}>
        <div className="card" style={{ padding: 20 }}>
          <label style={fieldLabel}>Title</label>
          <input
            style={fieldInput}
            value={form.title}
            onChange={(e) => update('title', e.target.value)}
            onBlur={() => !form.slug && update('slug', slugify(form.title))}
          />

          <label style={fieldLabel}>Slug</label>
          <input style={fieldInput} value={form.slug} onChange={(e) => update('slug', slugify(e.target.value))} placeholder="auto-generated-from-title" />

          <label style={fieldLabel}>Short Description</label>
          <textarea style={{ ...fieldInput, minHeight: 70 }} value={form.shortDescription} onChange={(e) => update('shortDescription', e.target.value)} />

          <label style={fieldLabel}>Content</label>
          <RichTextEditor onUploadingChange={setInlineUploading} value={form.content} onChange={(html) => update('content', html)} />

          <div style={{ marginTop: 60, display: 'flex', gap: 10 }}>
            <button className="btn-outline" disabled={saving || uploading || inlineUploading || loadingArticle || loadError} onClick={() => handleSave('Draft')}>Save Draft</button>
            <button className="btn-primary" disabled={saving || uploading || inlineUploading || loadingArticle || loadError} onClick={() => handleSave('Published')}>Publish</button>
          </div>
        </div>

        <div>
          <div className="card" style={{ padding: 20, marginBottom: 20 }}>
            <label style={fieldLabel}>Featured Image</label>
            <ImageUploader onUploadingChange={setUploading} value={form.featuredImageUrl} onChange={(url) => update('featuredImageUrl', url)} />

            <label style={fieldLabel}>Category</label>
            <select style={fieldInput} value={form.categoryId} onChange={(e) => update('categoryId', Number(e.target.value))}>
              <option value="">Select category…</option>
              {categories?.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>

            <label style={fieldLabel}>Movie (optional — for box office posts)</label>
            <select style={fieldInput} value={form.movieId} onChange={(e) => update('movieId', e.target.value ? Number(e.target.value) : '')}>
              <option value="">None</option>
              {movies?.items.map((m) => <option key={m.id} value={m.id}>{m.movieName}</option>)}
            </select>

            <label style={fieldLabel}>Schedule (optional)</label>
            <input type="datetime-local" style={fieldInput} value={form.publishedDate} onChange={(e) => update('publishedDate', e.target.value)} />

            <label style={{ display: 'flex', alignItems: 'center', gap: 8, marginTop: 12 }}>
              <input type="checkbox" checked={form.isFeatured} onChange={(e) => update('isFeatured', e.target.checked)} /> Featured
            </label>
            <label style={{ display: 'flex', alignItems: 'center', gap: 8, marginTop: 6 }}>
              <input type="checkbox" checked={form.isTrending} onChange={(e) => update('isTrending', e.target.checked)} /> Trending
            </label>
          </div>

          <div className="card" style={{ padding: 20 }}>
            <h4 style={{ marginTop: 0 }}>SEO</h4>
            <label style={fieldLabel}>SEO Title</label>
            <input style={fieldInput} value={form.seoTitle} onChange={(e) => update('seoTitle', e.target.value)} />
            <label style={fieldLabel}>SEO Description</label>
            <textarea style={{ ...fieldInput, minHeight: 60 }} value={form.seoDescription} onChange={(e) => update('seoDescription', e.target.value)} />
            <label style={fieldLabel}>SEO Keywords</label>
            <input style={fieldInput} value={form.seoKeywords} onChange={(e) => update('seoKeywords', e.target.value)} placeholder="comma, separated" />
          </div>
        </div>
      </div>

      {toast && <Toast message={toast.message} type={toast.type} onDismiss={() => setToast(null)} />}
    </div>
  );
}

const fieldLabel: CSSProperties = { display: 'block', fontSize: 12, fontWeight: 700, color: '#666', margin: '14px 0 6px', textTransform: 'uppercase' };
const fieldInput: CSSProperties = { width: '100%', padding: 10, border: '1px solid var(--n4n-border)', borderRadius: 6, fontSize: 14, fontFamily: 'inherit' };
