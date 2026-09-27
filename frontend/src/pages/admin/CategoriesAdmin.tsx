import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { categoriesApi } from '../../api/categories';
import DataTable from '../../components/admin/DataTable';
import Modal from '../../components/common/Modal';
import Toast from '../../components/common/Toast';
import { Loading, ErrorState } from '../../components/common/StateViews';

export default function CategoriesAdmin() {
  const qc = useQueryClient();
  const { data, isLoading, isError } = useQuery({ queryKey: ['admin-categories'], queryFn: () => categoriesApi.getAll(false) });

  const [modalOpen, setModalOpen] = useState(false);
  const [name, setName] = useState('');
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);

  const create = useMutation({
    mutationFn: () => categoriesApi.create({ name, isActive: true, displayOrder: (data?.length ?? 0) + 1 }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin-categories'] });
      setModalOpen(false);
      setName('');
      setToast({ message: 'Category created.', type: 'success' });
    },
  });

  const toggleActive = useMutation({
    mutationFn: (vars: { id: number; name: string; isActive: boolean; displayOrder: number }) =>
      categoriesApi.update(vars.id, { name: vars.name, isActive: !vars.isActive, displayOrder: vars.displayOrder }),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['admin-categories'] }),
  });

  const remove = useMutation({
    mutationFn: (id: number) => categoriesApi.remove(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['admin-categories'] }),
  });

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <h1 style={{ margin: 0 }}>Categories</h1>
        <button className="btn-primary" onClick={() => setModalOpen(true)}>+ New Category</button>
      </div>

      {isLoading && <Loading />}
      {isError && <ErrorState />}
      {data && (
        <DataTable
          keyFn={(c) => c.id}
          rows={data}
          columns={[
            { header: 'Name', render: (c) => c.name },
            { header: 'Slug', render: (c) => c.slug },
            { header: 'Articles', render: (c) => c.articleCount },
            { header: 'Status', render: (c) => (c.isActive ? 'Active' : 'Inactive') },
            {
              header: 'Actions',
              render: (c) => (
                <div style={{ display: 'flex', gap: 8 }}>
                  <button className="btn-outline" onClick={() => toggleActive.mutate(c)}>
                    {c.isActive ? 'Deactivate' : 'Activate'}
                  </button>
                  <button
                    className="btn-outline"
                    onClick={() => {
                      if (confirm(`Delete category "${c.name}"?`)) remove.mutate(c.id);
                    }}
                  >
                    Delete
                  </button>
                </div>
              ),
            },
          ]}
        />
      )}

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title="New Category">
        <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
          <input
            placeholder="Category name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            style={{ padding: 10, border: '1px solid var(--n4n-border)', borderRadius: 6 }}
          />
          <button className="btn-primary" disabled={!name || create.isPending} onClick={() => create.mutate()}>
            {create.isPending ? 'Saving…' : 'Create'}
          </button>
        </div>
      </Modal>

      {toast && <Toast message={toast.message} type={toast.type} onDismiss={() => setToast(null)} />}
    </div>
  );
}
