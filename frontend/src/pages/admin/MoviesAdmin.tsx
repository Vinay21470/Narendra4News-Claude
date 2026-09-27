import { useState, type CSSProperties } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { moviesApi } from '../../api/movies';
import DataTable from '../../components/admin/DataTable';
import Modal from '../../components/common/Modal';
import ImageUploader from '../../components/admin/ImageUploader';
import { Loading, ErrorState } from '../../components/common/StateViews';
import { formatDate, slugify } from '../../utils/format';
import Toast from '../../components/common/Toast';

interface MovieForm {
  movieName: string;
  posterUrl: string;
  heroImageUrl: string;
  description: string;
  releaseDate: string;
  hero: string;
  director: string;
  producer: string;
  productionHouse: string;
  genre: string;
  language: string;
  runtimeMinutes: string;
  certification: string;
}
const EMPTY_MOVIE: MovieForm = {
  movieName: '', posterUrl: '', heroImageUrl: '', description: '', releaseDate: '',
  hero: '', director: '', producer: '', productionHouse: '', genre: '', language: 'Telugu',
  runtimeMinutes: '', certification: '',
};

interface CollectionForm {
  dayNumber: string;
  label: string;
  collectionDate: string;
  indiaNet: string;
  indiaGross: string;
  overseas: string;
  worldwideGross: string;
  openingDay: string;
  weekendCollection: string;
  totalCollection: string;
}
const EMPTY_COLLECTION: CollectionForm = {
  dayNumber: '', label: '', collectionDate: new Date().toISOString().slice(0, 10),
  indiaNet: '', indiaGross: '', overseas: '', worldwideGross: '', openingDay: '', weekendCollection: '', totalCollection: '',
};

export default function MoviesAdmin() {
  const qc = useQueryClient();
  const { data, isLoading, isError } = useQuery({ queryKey: ['admin-movies'], queryFn: () => moviesApi.getAll(1, 100) });

  const [movieModalOpen, setMovieModalOpen] = useState(false);
  const [movieForm, setMovieForm] = useState<MovieForm>(EMPTY_MOVIE);

  const [collectionModalOpen, setCollectionModalOpen] = useState(false);
  const [collectionMovieId, setCollectionMovieId] = useState<number | null>(null);
  const [collectionForm, setCollectionForm] = useState<CollectionForm>(EMPTY_COLLECTION);

  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);

  const createMovie = useMutation({
    mutationFn: () =>
      moviesApi.create({
        movieName: movieForm.movieName,
        slug: slugify(movieForm.movieName),
        posterUrl: movieForm.posterUrl || undefined,
        heroImageUrl: movieForm.heroImageUrl || undefined,
        description: movieForm.description || undefined,
        releaseDate: movieForm.releaseDate ? new Date(movieForm.releaseDate).toISOString() : undefined,
        hero: movieForm.hero || undefined,
        director: movieForm.director || undefined,
        producer: movieForm.producer || undefined,
        productionHouse: movieForm.productionHouse || undefined,
        genre: movieForm.genre || undefined,
        language: movieForm.language || undefined,
        runtimeMinutes: movieForm.runtimeMinutes ? Number(movieForm.runtimeMinutes) : undefined,
        certification: movieForm.certification || undefined,
      }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin-movies'] });
      setMovieModalOpen(false);
      setMovieForm(EMPTY_MOVIE);
      setToast({ message: 'Movie created successfully.', type: 'success' });
    },
    onError: () => setToast({ message: 'Failed to create movie.', type: 'error' }),
  });

  const addCollection = useMutation({
    mutationFn: () => {
      if (!collectionMovieId) throw new Error('no movie');
      return moviesApi.addCollection(collectionMovieId, {
        dayNumber: collectionForm.dayNumber ? Number(collectionForm.dayNumber) : undefined,
        label: collectionForm.label || undefined,
        collectionDate: new Date(collectionForm.collectionDate).toISOString(),
        indiaNet: collectionForm.indiaNet ? Number(collectionForm.indiaNet) : undefined,
        indiaGross: collectionForm.indiaGross ? Number(collectionForm.indiaGross) : undefined,
        overseas: collectionForm.overseas ? Number(collectionForm.overseas) : undefined,
        worldwideGross: collectionForm.worldwideGross ? Number(collectionForm.worldwideGross) : undefined,
        openingDay: collectionForm.openingDay ? Number(collectionForm.openingDay) : undefined,
        weekendCollection: collectionForm.weekendCollection ? Number(collectionForm.weekendCollection) : undefined,
        totalCollection: collectionForm.totalCollection ? Number(collectionForm.totalCollection) : undefined,
      });
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin-movies'] });
      setCollectionModalOpen(false);
      setCollectionForm(EMPTY_COLLECTION);
      setToast({ message: 'Collection record added. Previous days are kept intact.', type: 'success' });
    },
    onError: () => setToast({ message: 'Failed to add collection record.', type: 'error' }),
  });

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <h1 style={{ margin: 0 }}>Movies</h1>
        <button className="btn-primary" onClick={() => setMovieModalOpen(true)}>+ New Movie</button>
      </div>

      {isLoading && <Loading />}
      {isError && <ErrorState />}
      {data && (
        <DataTable
          keyFn={(m) => m.id}
          rows={data.items}
          columns={[
            { header: 'Movie', render: (m) => m.movieName },
            { header: 'Genre', render: (m) => m.genre ?? '—' },
            { header: 'Release Date', render: (m) => formatDate(m.releaseDate) || '—' },
            {
              header: 'Actions',
              render: (m) => (
                <button
                  className="btn-outline"
                  onClick={() => {
                    setCollectionMovieId(m.id);
                    setCollectionModalOpen(true);
                  }}
                >
                  + Add Collection
                </button>
              ),
            },
          ]}
        />
      )}

      <Modal open={movieModalOpen} onClose={() => setMovieModalOpen(false)} title="New Movie">
        <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
          <input placeholder="Movie Name" value={movieForm.movieName} onChange={(e) => setMovieForm({ ...movieForm, movieName: e.target.value })} style={input} />
          <ImageUploader value={movieForm.posterUrl} onChange={(url) => setMovieForm({ ...movieForm, posterUrl: url })} />
          <textarea placeholder="Description" value={movieForm.description} onChange={(e) => setMovieForm({ ...movieForm, description: e.target.value })} style={{ ...input, minHeight: 60 }} />
          <input type="date" value={movieForm.releaseDate} onChange={(e) => setMovieForm({ ...movieForm, releaseDate: e.target.value })} style={input} />
          <input placeholder="Hero" value={movieForm.hero} onChange={(e) => setMovieForm({ ...movieForm, hero: e.target.value })} style={input} />
          <input placeholder="Director" value={movieForm.director} onChange={(e) => setMovieForm({ ...movieForm, director: e.target.value })} style={input} />
          <input placeholder="Producer" value={movieForm.producer} onChange={(e) => setMovieForm({ ...movieForm, producer: e.target.value })} style={input} />
          <input placeholder="Production House" value={movieForm.productionHouse} onChange={(e) => setMovieForm({ ...movieForm, productionHouse: e.target.value })} style={input} />
          <input placeholder="Genre" value={movieForm.genre} onChange={(e) => setMovieForm({ ...movieForm, genre: e.target.value })} style={input} />
          <input placeholder="Language" value={movieForm.language} onChange={(e) => setMovieForm({ ...movieForm, language: e.target.value })} style={input} />
          <button className="btn-primary" disabled={!movieForm.movieName || createMovie.isPending} onClick={() => createMovie.mutate()}>
            {createMovie.isPending ? 'Saving…' : 'Create Movie'}
          </button>
        </div>
      </Modal>

      <Modal open={collectionModalOpen} onClose={() => setCollectionModalOpen(false)} title="Add Box Office Collection">
        <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
          <input type="date" value={collectionForm.collectionDate} onChange={(e) => setCollectionForm({ ...collectionForm, collectionDate: e.target.value })} style={input} />
          <input placeholder="Day Number (e.g. 1)" value={collectionForm.dayNumber} onChange={(e) => setCollectionForm({ ...collectionForm, dayNumber: e.target.value })} style={input} />
          <input placeholder="Label (e.g. Weekend, Week 1)" value={collectionForm.label} onChange={(e) => setCollectionForm({ ...collectionForm, label: e.target.value })} style={input} />
          <input placeholder="Opening Day (Cr)" value={collectionForm.openingDay} onChange={(e) => setCollectionForm({ ...collectionForm, openingDay: e.target.value })} style={input} />
          <input placeholder="India Net (Cr)" value={collectionForm.indiaNet} onChange={(e) => setCollectionForm({ ...collectionForm, indiaNet: e.target.value })} style={input} />
          <input placeholder="India Gross (Cr)" value={collectionForm.indiaGross} onChange={(e) => setCollectionForm({ ...collectionForm, indiaGross: e.target.value })} style={input} />
          <input placeholder="Overseas (Cr)" value={collectionForm.overseas} onChange={(e) => setCollectionForm({ ...collectionForm, overseas: e.target.value })} style={input} />
          <input placeholder="Weekend Total (Cr)" value={collectionForm.weekendCollection} onChange={(e) => setCollectionForm({ ...collectionForm, weekendCollection: e.target.value })} style={input} />
          <input placeholder="Worldwide (Cr)" value={collectionForm.worldwideGross} onChange={(e) => setCollectionForm({ ...collectionForm, worldwideGross: e.target.value })} style={input} />
          <input placeholder="Total Collection (Cr)" value={collectionForm.totalCollection} onChange={(e) => setCollectionForm({ ...collectionForm, totalCollection: e.target.value })} style={input} />
          <button className="btn-primary" disabled={addCollection.isPending} onClick={() => addCollection.mutate()}>
            {addCollection.isPending ? 'Saving…' : 'Add Collection Record'}
          </button>
        </div>
      </Modal>

      {toast && <Toast message={toast.message} type={toast.type} onDismiss={() => setToast(null)} />}
    </div>
  );
}

const input: CSSProperties = { padding: 10, border: '1px solid var(--n4n-border)', borderRadius: 6, fontSize: 14, fontFamily: 'inherit' };
