import { Routes, Route } from 'react-router-dom';
import Header from './components/layout/Header';
import Footer from './components/layout/Footer';
import Home from './pages/Home';
import ArticleDetail from './pages/ArticleDetail';
import CategoryPage from './pages/CategoryPage';
import MoviesList from './pages/MoviesList';
import MovieDetail from './pages/MovieDetail';
import MovieBoxOffice from './pages/MovieBoxOffice';
import BoxOfficeList from './pages/BoxOfficeList';
import SearchPage from './pages/SearchPage';
import Login from './pages/Login';
import Register from './pages/Register';
import StaticPage from './pages/StaticPage';
import NotFound from './pages/NotFound';

import ProtectedRoute from './components/admin/ProtectedRoute';
import AdminLayout from './components/admin/AdminLayout';
import Dashboard from './pages/admin/Dashboard';
import ArticlesAdmin from './pages/admin/ArticlesAdmin';
import ArticleEditor from './pages/admin/ArticleEditor';
import MoviesAdmin from './pages/admin/MoviesAdmin';
import CategoriesAdmin from './pages/admin/CategoriesAdmin';
import MediaAdmin from './pages/admin/MediaAdmin';

function PublicLayout({ children }: { children: React.ReactNode }) {
  return (
    <>
      <Header />
      {children}
      <Footer />
    </>
  );
}

export default function App() {
  return (
    <Routes>
      {/* Public site */}
      <Route path="/" element={<PublicLayout><Home /></PublicLayout>} />
      <Route path="/article/:slug" element={<PublicLayout><ArticleDetail /></PublicLayout>} />
      <Route path="/category/:slug" element={<PublicLayout><CategoryPage /></PublicLayout>} />
      <Route path="/movies" element={<PublicLayout><MoviesList /></PublicLayout>} />
      <Route path="/movie/:slug" element={<PublicLayout><MovieDetail /></PublicLayout>} />
      <Route path="/movie/:slug/box-office" element={<PublicLayout><MovieBoxOffice /></PublicLayout>} />
      <Route path="/box-office" element={<PublicLayout><BoxOfficeList /></PublicLayout>} />
      <Route path="/box-office/:slug" element={<PublicLayout><MovieBoxOffice /></PublicLayout>} />
      <Route path="/search" element={<PublicLayout><SearchPage /></PublicLayout>} />
      <Route path="/login" element={<PublicLayout><Login /></PublicLayout>} />
      <Route path="/register" element={<PublicLayout><Register /></PublicLayout>} />
      <Route path="/about" element={<PublicLayout><StaticPage pageKey="about" /></PublicLayout>} />
      <Route path="/contact" element={<PublicLayout><StaticPage pageKey="contact" /></PublicLayout>} />
      <Route path="/privacy-policy" element={<PublicLayout><StaticPage pageKey="privacy-policy" /></PublicLayout>} />
      <Route path="/terms" element={<PublicLayout><StaticPage pageKey="terms" /></PublicLayout>} />
      <Route path="/disclaimer" element={<PublicLayout><StaticPage pageKey="disclaimer" /></PublicLayout>} />

      {/* Admin */}
      <Route path="/admin" element={<ProtectedRoute><AdminLayout><Dashboard /></AdminLayout></ProtectedRoute>} />
      <Route path="/admin/articles" element={<ProtectedRoute><AdminLayout><ArticlesAdmin /></AdminLayout></ProtectedRoute>} />
      <Route path="/admin/articles/create" element={<ProtectedRoute><AdminLayout><ArticleEditor /></AdminLayout></ProtectedRoute>} />
      <Route path="/admin/articles/edit/:id" element={<ProtectedRoute><AdminLayout><ArticleEditor /></AdminLayout></ProtectedRoute>} />
      <Route path="/admin/movies" element={<ProtectedRoute><AdminLayout><MoviesAdmin /></AdminLayout></ProtectedRoute>} />
      <Route path="/admin/categories" element={<ProtectedRoute><AdminLayout><CategoriesAdmin /></AdminLayout></ProtectedRoute>} />
      <Route path="/admin/media" element={<ProtectedRoute><AdminLayout><MediaAdmin /></AdminLayout></ProtectedRoute>} />

      <Route path="*" element={<PublicLayout><NotFound /></PublicLayout>} />
    </Routes>
  );
}
