import Seo from '../components/common/Seo';

const CONTENT: Record<string, { title: string; body: string }> = {
  about: { title: 'About Narendra4News', body: 'Narendra4News is a Telugu movie news and entertainment portal covering box office collections, reviews, gossips and industry updates.' },
  contact: { title: 'Contact Us', body: 'Reach the Narendra4News editorial team at contact@narendra4news.net.' },
  'privacy-policy': { title: 'Privacy Policy', body: 'This page describes how Narendra4News collects and uses information. Replace with your finalized policy before going live.' },
  terms: { title: 'Terms of Use', body: 'These are the terms governing use of Narendra4News. Replace with your finalized terms before going live.' },
  disclaimer: { title: 'Disclaimer', body: 'Box office figures and news reported on Narendra4News are compiled from trade sources and may vary from official numbers.' },
};

export default function StaticPage({ pageKey }: { pageKey: keyof typeof CONTENT }) {
  const page = CONTENT[pageKey];
  return (
    <div className="container" style={{ maxWidth: 720, padding: '40px 16px 60px' }}>
      <Seo title={page.title} />
      <h1>{page.title}</h1>
      <p style={{ lineHeight: 1.7, color: '#444' }}>{page.body}</p>
    </div>
  );
}
