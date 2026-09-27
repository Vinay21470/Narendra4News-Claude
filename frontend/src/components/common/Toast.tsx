import { useEffect } from 'react';

interface Props {
  message: string;
  type?: 'success' | 'error';
  onDismiss: () => void;
}

export default function Toast({ message, type = 'success', onDismiss }: Props) {
  useEffect(() => {
    const t = setTimeout(onDismiss, 3500);
    return () => clearTimeout(t);
  }, [onDismiss]);

  return (
    <div
      style={{
        position: 'fixed', bottom: 20, right: 20, zIndex: 200,
        background: type === 'success' ? '#2e7d32' : '#c62828',
        color: '#fff', padding: '12px 18px', borderRadius: 6,
        boxShadow: '0 2px 10px rgba(0,0,0,0.2)', fontSize: 14, fontWeight: 600,
      }}
      onClick={onDismiss}
      role="alert"
    >
      {message}
    </div>
  );
}
