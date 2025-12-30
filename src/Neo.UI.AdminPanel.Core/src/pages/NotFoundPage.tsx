/**
 * 404 Not Found Page
 * Neo BPMS Admin Panel Core
 */

import { useNavigate } from 'react-router-dom';
import clsx from 'clsx';

export interface NotFoundPageProps {
  /** Title text */
  title?: string;
  /** Description text */
  description?: string;
  /** Show back button */
  showBackButton?: boolean;
  /** Show home button */
  showHomeButton?: boolean;
  /** Home path */
  homePath?: string;
  /** Custom illustration */
  illustration?: React.ReactNode;
  /** Custom class name */
  className?: string;
}

/**
 * 404 Not Found Page Component
 */
export function NotFoundPage({
  title = 'صفحه پیدا نشد',
  description = 'صفحه‌ای که به دنبال آن هستید وجود ندارد یا منتقل شده است.',
  showBackButton = true,
  showHomeButton = true,
  homePath = '/',
  illustration,
  className,
}: NotFoundPageProps) {
  const navigate = useNavigate();

  return (
    <div
      className={clsx(
        'min-h-screen flex items-center justify-center p-4',
        'bg-gradient-to-br from-gray-50 to-gray-100 dark:from-gray-900 dark:to-gray-800',
        className
      )}
    >
      <div className="text-center max-w-md">
        {/* Illustration */}
        {illustration || (
          <div className="relative mb-8">
            <div className="text-[150px] font-black text-gray-200 dark:text-gray-700 leading-none select-none">
              404
            </div>
            <div className="absolute inset-0 flex items-center justify-center">
              <svg
                className="w-32 h-32 text-purple-500 opacity-80"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={1.5}
                  d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
            </div>
          </div>
        )}

        {/* Title */}
        <h1 className="text-3xl font-bold text-gray-900 dark:text-white mb-3">
          {title}
        </h1>

        {/* Description */}
        <p className="text-gray-500 dark:text-gray-400 mb-8">
          {description}
        </p>

        {/* Buttons */}
        <div className="flex flex-col sm:flex-row items-center justify-center gap-3">
          {showBackButton && (
            <button
              onClick={() => navigate(-1)}
              className={clsx(
                'px-6 py-2.5 rounded-xl font-medium',
                'border border-gray-300 dark:border-gray-600',
                'text-gray-700 dark:text-gray-300',
                'hover:bg-gray-50 dark:hover:bg-gray-800',
                'transition-colors duration-200',
                'flex items-center gap-2'
              )}
            >
              <svg className="w-5 h-5 rotate-180" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M14 5l7 7m0 0l-7 7m7-7H3" />
              </svg>
              بازگشت
            </button>
          )}

          {showHomeButton && (
            <button
              onClick={() => navigate(homePath)}
              className={clsx(
                'px-6 py-2.5 rounded-xl font-medium',
                'bg-gradient-to-r from-purple-600 to-pink-600',
                'text-white',
                'hover:from-purple-500 hover:to-pink-500',
                'shadow-lg shadow-purple-500/30',
                'transition-all duration-200',
                'flex items-center gap-2'
              )}
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
              </svg>
              صفحه اصلی
            </button>
          )}
        </div>
      </div>
    </div>
  );
}

export default NotFoundPage;

