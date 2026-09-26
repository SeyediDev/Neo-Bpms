/**
 * Login Page Component
 * Neo BPMS Admin Panel Core
 * 
 * A modern, beautiful login page with RTL support.
 */

import { useState, useCallback, type FormEvent } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import clsx from 'clsx';
import { useAuthContext } from '../auth/AuthProvider';

export interface LoginPageProps {
  /** Logo component or image URL */
  logo?: React.ReactNode | string;
  /** Application title */
  title?: string;
  /** Subtitle or description */
  subtitle?: string;
  /** Background image or gradient */
  backgroundImage?: string;
  /** Custom footer content */
  footer?: React.ReactNode;
  /** Path to redirect after login */
  redirectPath?: string;
  /** Show remember me checkbox */
  showRememberMe?: boolean;
  /** Show forgot password link */
  showForgotPassword?: boolean;
  /** Forgot password URL */
  forgotPasswordUrl?: string;
  /** Custom class name */
  className?: string;
  /** On login success callback */
  onLoginSuccess?: () => void;
  /** On login error callback */
  onLoginError?: (error: string) => void;
}

/**
 * Modern Login Page Component
 */
export function LoginPage({
  logo,
  title = 'پنل مدیریت',
  subtitle = 'وارد حساب کاربری خود شوید',
  backgroundImage,
  footer,
  redirectPath = '/',
  showRememberMe = true,
  showForgotPassword = true,
  forgotPasswordUrl = '/forgot-password',
  className,
  onLoginSuccess,
  onLoginError,
}: LoginPageProps) {
  const navigate = useNavigate();
  const location = useLocation();
  const { login, isLoading, error: authError } = useAuthContext();

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [rememberMe, setRememberMe] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [localError, setLocalError] = useState<string | null>(null);

  // Get redirect path from location state or use default
  const from = (location.state as { from?: { pathname: string } })?.from?.pathname || redirectPath;

  /**
   * Handle form submission
   */
  const handleSubmit = useCallback(async (e: FormEvent) => {
    e.preventDefault();
    setLocalError(null);

    // Validation
    if (!username.trim()) {
      setLocalError('نام کاربری را وارد کنید');
      return;
    }

    if (!password) {
      setLocalError('رمز عبور را وارد کنید');
      return;
    }

    try {
      await login({
        username: username.trim(),
        password,
        rememberMe,
      });

      onLoginSuccess?.();
      navigate(from, { replace: true });
    } catch (err) {
      const message = err instanceof Error ? err.message : 'خطا در ورود';
      setLocalError(message);
      onLoginError?.(message);
    }
  }, [username, password, rememberMe, login, navigate, from, onLoginSuccess, onLoginError]);

  const error = localError || authError;

  return (
    <div
      className={clsx(
        'min-h-screen flex items-center justify-center',
        'bg-canvas',
        'px-4 py-8',
        className
      )}
      style={backgroundImage ? { backgroundImage: `url(${backgroundImage})`, backgroundSize: 'cover' } : undefined}
    >
      {/* Decorative elements */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <div className="absolute -top-40 -right-40 w-80 h-80 bg-purple-500 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob" />
        <div className="absolute -bottom-40 -left-40 w-80 h-80 bg-cyan-500 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob animation-delay-2000" />
        <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-80 h-80 bg-pink-500 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob animation-delay-4000" />
      </div>

      {/* Login Card */}
      <div className="relative w-full max-w-md">
        <div className="backdrop-blur-xl bg-surface rounded-3xl shadow-2xl border border-border p-8">
          {/* Logo & Title */}
          <div className="text-center mb-8">
            {logo && (
              <div className="mb-4 flex justify-center">
                {typeof logo === 'string' ? (
                  <img src={logo} alt="Logo" className="h-16 w-auto" />
                ) : (
                  logo
                )}
              </div>
            )}
            <h1 className="text-3xl font-bold text-text mb-2">{title}</h1>
            <p className="text-subtle">{subtitle}</p>
          </div>

          {/* Error Alert */}
          {error && (
            <div className="mb-6 p-4 bg-red-500/20 border border-red-500/30 rounded-xl text-red-700 dark:text-red-200 text-sm text-center backdrop-blur-sm">
              <svg className="inline-block w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              {error}
            </div>
          )}

          {/* Login Form */}
          <form onSubmit={handleSubmit} className="space-y-5">
            {/* Username Field */}
            <div>
              <label htmlFor="username" className="block text-sm font-medium text-subtle mb-2">
                نام کاربری
              </label>
              <div className="relative">
                <input
                  id="username"
                  type="text"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                  className={clsx(
                    'w-full px-4 py-3 pr-11',
                    'bg-surface backdrop-blur-sm',
                    'border border-border rounded-xl',
                    'text-text placeholder-muted',
                    'focus:outline-none focus:ring-2 focus:ring-accent focus:border-transparent',
                    'transition-all duration-200'
                  )}
                  placeholder="نام کاربری یا ایمیل"
                  autoComplete="username"
                  disabled={isLoading}
                />
                <svg
                  className="absolute right-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                </svg>
              </div>
            </div>

            {/* Password Field */}
            <div>
              <label htmlFor="password" className="block text-sm font-medium text-subtle mb-2">
                رمز عبور
              </label>
              <div className="relative">
                <input
                  id="password"
                  type={showPassword ? 'text' : 'password'}
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  className={clsx(
                    'w-full px-4 py-3 pr-11 pl-11',
                    'bg-surface backdrop-blur-sm',
                    'border border-border rounded-xl',
                    'text-text placeholder-muted',
                    'focus:outline-none focus:ring-2 focus:ring-accent focus:border-transparent',
                    'transition-all duration-200'
                  )}
                  placeholder="رمز عبور"
                  autoComplete="current-password"
                  disabled={isLoading}
                />
                <svg
                  className="absolute right-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                </svg>
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute left-3 top-1/2 -translate-y-1/2 text-muted hover:text-text transition-colors"
                  tabIndex={-1}
                >
                  {showPassword ? (
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
                    </svg>
                  ) : (
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                    </svg>
                  )}
                </button>
              </div>
            </div>

            {/* Remember Me & Forgot Password */}
            <div className="flex items-center justify-between">
              {showRememberMe && (
                <label className="flex items-center gap-2 cursor-pointer group">
                  <input
                    type="checkbox"
                    checked={rememberMe}
                    onChange={(e) => setRememberMe(e.target.checked)}
                    className="w-4 h-4 rounded border-border bg-surface text-accent focus:ring-accent focus:ring-offset-0"
                    disabled={isLoading}
                  />
                  <span className="text-sm text-subtle group-hover:text-text transition-colors">
                    مرا به خاطر بسپار
                  </span>
                </label>
              )}

              {showForgotPassword && (
                <a
                  href={forgotPasswordUrl}
                  className="text-sm text-accent hover:text-accent-hover transition-colors"
                >
                  فراموشی رمز عبور؟
                </a>
              )}
            </div>

            {/* Submit Button */}
            <button
              type="submit"
              disabled={isLoading}
              className={clsx(
                'w-full py-3 px-4 rounded-xl font-medium text-on-accent',
                'bg-gradient-to-r from-accent to-accent-hover',
                'hover:from-accent-hover hover:to-accent',
                'focus:outline-none focus:ring-2 focus:ring-accent focus:ring-offset-2 focus:ring-offset-transparent',
                'transition-all duration-200',
                'shadow-lg shadow-purple-500/30',
                isLoading && 'opacity-75 cursor-not-allowed'
              )}
            >
              {isLoading ? (
                <span className="flex items-center justify-center gap-2">
                  <svg className="animate-spin h-5 w-5" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
                  </svg>
                  در حال ورود...
                </span>
              ) : (
                'ورود به سیستم'
              )}
            </button>
          </form>

          {/* Footer */}
          {footer && (
            <div className="mt-8 text-center text-sm text-muted">
              {footer}
            </div>
          )}
        </div>

        {/* Version / Copyright */}
        <p className="mt-6 text-center text-xs text-muted">
          © {new Date().getFullYear()} Neo BPMS. تمام حقوق محفوظ است.
        </p>
      </div>

      {/* Custom styles for animations */}
      <style>{`
        @keyframes blob {
          0%, 100% { transform: translate(0, 0) scale(1); }
          25% { transform: translate(20px, -30px) scale(1.1); }
          50% { transform: translate(-20px, 20px) scale(0.9); }
          75% { transform: translate(30px, 30px) scale(1.05); }
        }
        .animate-blob {
          animation: blob 7s infinite;
        }
        .animation-delay-2000 {
          animation-delay: 2s;
        }
        .animation-delay-4000 {
          animation-delay: 4s;
        }
      `}</style>
    </div>
  );
}

export default LoginPage;

