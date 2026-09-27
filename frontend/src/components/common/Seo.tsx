import { Helmet } from 'react-helmet-async';

interface Props {
  title: string;
  description?: string;
  keywords?: string;
  image?: string;
  url?: string;
  type?: 'website' | 'article';
  jsonLd?: Record<string, unknown>;
}

// Centralized SEO tags: title, meta description, canonical, Open Graph,
// Twitter Card, and optional JSON-LD structured data (spec section 14).
export default function Seo({ title, description, keywords, image, url, type = 'website', jsonLd }: Props) {
  const fullTitle = `${title} | Narendra4News`;
  const canonical = url ?? (typeof window !== 'undefined' ? window.location.href : undefined);

  return (
    <Helmet>
      <title>{fullTitle}</title>
      {description && <meta name="description" content={description} />}
      {keywords && <meta name="keywords" content={keywords} />}
      {canonical && <link rel="canonical" href={canonical} />}

      <meta property="og:title" content={fullTitle} />
      {description && <meta property="og:description" content={description} />}
      <meta property="og:type" content={type} />
      {canonical && <meta property="og:url" content={canonical} />}
      {image && <meta property="og:image" content={image} />}
      <meta property="og:site_name" content="Narendra4News" />

      <meta name="twitter:card" content={image ? 'summary_large_image' : 'summary'} />
      <meta name="twitter:title" content={fullTitle} />
      {description && <meta name="twitter:description" content={description} />}
      {image && <meta name="twitter:image" content={image} />}

      {jsonLd && <script type="application/ld+json">{JSON.stringify(jsonLd)}</script>}
    </Helmet>
  );
}
