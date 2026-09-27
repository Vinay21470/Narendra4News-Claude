// Small shared loading / error / empty state components used across pages
// (spec section 23: consistent Loading / Error / Empty / Success states).
export function Loading({ label = 'Loading…' }: { label?: string }) {
  return <div className="loading-state">{label}</div>;
}

export function ErrorState({ message = 'Something went wrong. Please try again.' }: { message?: string }) {
  return <div className="error-state">{message}</div>;
}

export function Empty({ message = 'Nothing to show yet.' }: { message?: string }) {
  return <div className="empty-state">{message}</div>;
}
