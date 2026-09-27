export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data: T;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface ArticleListItem {
  id: number;
  title: string;
  slug: string;
  shortDescription: string;
  featuredImageUrl?: string;
  categoryName: string;
  categorySlug: string;
  publishedDate?: string;
  views: number;
  isFeatured: boolean;
  isTrending: boolean;
}

export interface ArticleDetail extends ArticleListItem {
  categoryId: number;
  movieId?: number;
  status: 'Draft' | 'Published' | 'Archived' | 'Scheduled';
  content: string;
  authorName: string;
  updatedDate?: string;
  likes: number;
  seoTitle?: string;
  seoDescription?: string;
  seoKeywords?: string;
  movieSlug?: string;
  relatedArticles: ArticleListItem[];
}

export interface Category {
  id: number;
  name: string;
  slug: string;
  description?: string;
  displayOrder: number;
  isActive: boolean;
  articleCount: number;
}

export interface MovieListItem {
  id: number;
  movieName: string;
  slug: string;
  posterUrl?: string;
  releaseDate?: string;
  genre?: string;
}

export interface MovieCollectionRecord {
  id: number;
  collectionDate: string;
  dayNumber?: number;
  label?: string;
  indiaNet?: number;
  indiaGross?: number;
  overseas?: number;
  worldwideGross?: number;
  openingDay?: number;
  weekendCollection?: number;
  totalCollection?: number;
  notes?: string;
}

export interface MovieDetail extends MovieListItem {
  heroImageUrl?: string;
  description?: string;
  hero?: string;
  director?: string;
  producer?: string;
  productionHouse?: string;
  language?: string;
  budget?: number;
  runtimeMinutes?: number;
  certification?: string;
  collections: MovieCollectionRecord[];
}

export interface SearchResult {
  type: 'Article' | 'Movie';
  title: string;
  slug: string;
  imageUrl?: string;
  excerpt?: string;
  date?: string;
}

export interface CurrentUser {
  userId: string;
  email: string;
  displayName: string;
  roles: string[];
}

export interface DashboardStats {
  totalArticles: number;
  publishedArticles: number;
  draftArticles: number;
  totalMovies: number;
  totalUsers: number;
  totalViews: number;
  todaysViews: number;
  mostViewedArticleTitle?: string;
  mostViewedArticleSlug?: string;
}

export type ArticleStatus = 'Draft' | 'Published' | 'Archived' | 'Scheduled';
