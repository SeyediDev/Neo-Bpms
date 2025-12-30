/**
 * Avatar Component
 * Neo BPMS Admin Panel Core
 */

import { useState, type ImgHTMLAttributes } from 'react';
import clsx from 'clsx';

export type AvatarSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl' | '2xl';
export type AvatarShape = 'circle' | 'square';

export interface AvatarProps extends Omit<ImgHTMLAttributes<HTMLImageElement>, 'size'> {
  /** Image source */
  src?: string;
  /** Alt text / name for initials */
  name?: string;
  /** Avatar size */
  size?: AvatarSize;
  /** Avatar shape */
  shape?: AvatarShape;
  /** Show online indicator */
  online?: boolean;
  /** Custom background for initials */
  color?: string;
}

export interface AvatarGroupProps {
  /** Maximum visible avatars */
  max?: number;
  /** Avatar size */
  size?: AvatarSize;
  /** Children (Avatar components) */
  children: React.ReactNode;
  /** Custom class */
  className?: string;
}

const sizeClasses: Record<AvatarSize, { container: string; text: string; indicator: string }> = {
  xs: { container: 'w-6 h-6', text: 'text-xs', indicator: 'w-1.5 h-1.5' },
  sm: { container: 'w-8 h-8', text: 'text-sm', indicator: 'w-2 h-2' },
  md: { container: 'w-10 h-10', text: 'text-base', indicator: 'w-2.5 h-2.5' },
  lg: { container: 'w-12 h-12', text: 'text-lg', indicator: 'w-3 h-3' },
  xl: { container: 'w-16 h-16', text: 'text-xl', indicator: 'w-3.5 h-3.5' },
  '2xl': { container: 'w-20 h-20', text: 'text-2xl', indicator: 'w-4 h-4' },
};

// Generate consistent color from name
function stringToColor(str: string): string {
  const colors = [
    'bg-red-500', 'bg-orange-500', 'bg-amber-500', 'bg-yellow-500',
    'bg-lime-500', 'bg-green-500', 'bg-emerald-500', 'bg-teal-500',
    'bg-cyan-500', 'bg-sky-500', 'bg-blue-500', 'bg-indigo-500',
    'bg-violet-500', 'bg-purple-500', 'bg-fuchsia-500', 'bg-pink-500',
  ];
  
  let hash = 0;
  for (let i = 0; i < str.length; i++) {
    hash = str.charCodeAt(i) + ((hash << 5) - hash);
  }
  
  return colors[Math.abs(hash) % colors.length];
}

// Get initials from name
function getInitials(name: string): string {
  const parts = name.trim().split(/\s+/);
  if (parts.length >= 2) {
    return `${parts[0][0]}${parts[parts.length - 1][0]}`.toUpperCase();
  }
  return name.slice(0, 2).toUpperCase();
}

/**
 * Avatar Component
 */
export function Avatar({
  src,
  name,
  size = 'md',
  shape = 'circle',
  online,
  color,
  className,
  alt,
  ...props
}: AvatarProps) {
  const [imgError, setImgError] = useState(false);
  const sizes = sizeClasses[size];
  const showImage = src && !imgError;
  const initials = name ? getInitials(name) : '?';
  const bgColor = color || (name ? stringToColor(name) : 'bg-gray-400');

  return (
    <div className={clsx('relative inline-flex flex-shrink-0', className)}>
      <div
        className={clsx(
          'flex items-center justify-center overflow-hidden',
          sizes.container,
          shape === 'circle' ? 'rounded-full' : 'rounded-lg',
          !showImage && bgColor,
          !showImage && 'text-white font-medium'
        )}
      >
        {showImage ? (
          <img
            src={src}
            alt={alt || name || 'Avatar'}
            onError={() => setImgError(true)}
            className="w-full h-full object-cover"
            {...props}
          />
        ) : (
          <span className={sizes.text}>{initials}</span>
        )}
      </div>

      {/* Online indicator */}
      {online !== undefined && (
        <span
          className={clsx(
            'absolute bottom-0 left-0 block rounded-full ring-2 ring-white dark:ring-gray-800',
            sizes.indicator,
            online ? 'bg-emerald-500' : 'bg-gray-400'
          )}
        />
      )}
    </div>
  );
}

/**
 * Avatar Group Component
 */
export function AvatarGroup({
  max = 4,
  size = 'md',
  children,
  className,
}: AvatarGroupProps) {
  const avatars = Array.isArray(children) ? children : [children];
  const visible = avatars.slice(0, max);
  const remaining = avatars.length - max;
  const sizes = sizeClasses[size];

  return (
    <div className={clsx('flex -space-x-2 rtl:space-x-reverse', className)}>
      {visible.map((avatar, index) => (
        <div
          key={index}
          className="ring-2 ring-white dark:ring-gray-800 rounded-full"
          style={{ zIndex: visible.length - index }}
        >
          {avatar}
        </div>
      ))}

      {remaining > 0 && (
        <div
          className={clsx(
            'flex items-center justify-center rounded-full',
            'bg-gray-200 dark:bg-gray-700',
            'text-gray-600 dark:text-gray-300',
            'ring-2 ring-white dark:ring-gray-800',
            sizes.container,
            sizes.text,
            'font-medium'
          )}
        >
          +{remaining}
        </div>
      )}
    </div>
  );
}

export default Avatar;

