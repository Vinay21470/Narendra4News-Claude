interface Props {
  url: string;
  title: string;
}

// Share buttons per spec section 31.
export default function SocialShare({ url, title }: Props) {
  const encodedUrl = encodeURIComponent(url);
  const encodedTitle = encodeURIComponent(title);

  const links = [
    { label: 'WhatsApp', href: `https://wa.me/?text=${encodedTitle}%20${encodedUrl}` },
    { label: 'Facebook', href: `https://www.facebook.com/sharer/sharer.php?u=${encodedUrl}` },
    { label: 'X', href: `https://twitter.com/intent/tweet?text=${encodedTitle}&url=${encodedUrl}` },
    { label: 'Telegram', href: `https://t.me/share/url?url=${encodedUrl}&text=${encodedTitle}` },
  ];

  function copyLink() {
    navigator.clipboard?.writeText(url);
  }

  return (
    <div style={{ display: 'flex', gap: 10, flexWrap: 'wrap', margin: '16px 0' }}>
      {links.map((l) => (
        <a key={l.label} href={l.href} target="_blank" rel="noopener noreferrer" className="btn-outline">
          {l.label}
        </a>
      ))}
      <button className="btn-outline" onClick={copyLink}>Copy Link</button>
    </div>
  );
}
