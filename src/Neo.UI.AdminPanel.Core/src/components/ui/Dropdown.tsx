/**
 * Dropdown Component
 * Neo BPMS Admin Panel Core
 */

import { 
  useState, 
  useRef, 
  useEffect, 
  useCallback,
  type ReactNode,
  type MouseEvent,
} from 'react';
import { createPortal } from 'react-dom';
import clsx from 'clsx';

// ==================== Types ====================

export interface DropdownItem {
  key: string;
  label: ReactNode;
  icon?: ReactNode;
  disabled?: boolean;
  danger?: boolean;
  divider?: boolean;
}

export interface DropdownProps {
  /** Trigger element */
  trigger: ReactNode;
  /** Dropdown items */
  items: DropdownItem[];
  /** Item click handler */
  onItemClick?: (item: DropdownItem) => void;
  /** Placement */
  placement?: 'bottom-start' | 'bottom-end' | 'top-start' | 'top-end';
  /** Min width */
  minWidth?: number;
  /** Disabled state */
  disabled?: boolean;
  /** Custom class */
  className?: string;
}

export interface DropdownMenuProps {
  /** Open state */
  isOpen: boolean;
  /** Items */
  items: DropdownItem[];
  /** Click handler */
  onItemClick: (item: DropdownItem) => void;
  /** Close handler */
  onClose: () => void;
  /** Trigger element ref for positioning */
  triggerRef: React.RefObject<HTMLElement>;
  /** Placement */
  placement: 'bottom-start' | 'bottom-end' | 'top-start' | 'top-end';
  /** Min width */
  minWidth: number;
}

// ==================== Menu Component ====================

function DropdownMenu({
  isOpen,
  items,
  onItemClick,
  onClose,
  triggerRef,
  placement,
  minWidth,
}: DropdownMenuProps) {
  const menuRef = useRef<HTMLDivElement>(null);
  const [position, setPosition] = useState({ top: 0, left: 0 });

  // Calculate position
  useEffect(() => {
    if (!isOpen || !triggerRef.current) return;

    const trigger = triggerRef.current;
    const rect = trigger.getBoundingClientRect();
    const menuHeight = menuRef.current?.offsetHeight || 200;

    let top = 0;
    let left = 0;

    if (placement.startsWith('bottom')) {
      top = rect.bottom + 8;
    } else {
      top = rect.top - menuHeight - 8;
    }

    if (placement.endsWith('start')) {
      left = rect.left;
    } else {
      left = rect.right - (menuRef.current?.offsetWidth || minWidth);
    }

    // Keep in viewport
    const viewportHeight = window.innerHeight;
    const viewportWidth = window.innerWidth;

    if (top + menuHeight > viewportHeight) {
      top = rect.top - menuHeight - 8;
    }
    if (left < 0) left = 8;
    if (left + minWidth > viewportWidth) left = viewportWidth - minWidth - 8;

    setPosition({ top, left });
  }, [isOpen, triggerRef, placement, minWidth]);

  // Close on click outside
  useEffect(() => {
    if (!isOpen) return;

    const handleClickOutside = (e: Event) => {
      if (
        menuRef.current &&
        !menuRef.current.contains(e.target as Node) &&
        triggerRef.current &&
        !triggerRef.current.contains(e.target as Node)
      ) {
        onClose();
      }
    };

    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === 'Escape') onClose();
    };

    document.addEventListener('mousedown', handleClickOutside);
    document.addEventListener('keydown', handleEscape);

    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
      document.removeEventListener('keydown', handleEscape);
    };
  }, [isOpen, onClose, triggerRef]);

  if (!isOpen) return null;

  return createPortal(
    <div
      ref={menuRef}
      className={clsx(
        'fixed z-50 py-1',
        'bg-white dark:bg-gray-800',
        'border border-gray-200 dark:border-gray-700',
        'rounded-xl shadow-lg',
        'animate-in fade-in zoom-in-95 duration-150'
      )}
      style={{
        top: position.top,
        left: position.left,
        minWidth,
      }}
    >
      {items.map((item) => {
        if (item.divider) {
          return (
            <hr
              key={item.key}
              className="my-1 border-gray-200 dark:border-gray-700"
            />
          );
        }

        return (
          <button
            key={item.key}
            disabled={item.disabled}
            onClick={() => onItemClick(item)}
            className={clsx(
              'w-full flex items-center gap-2 px-4 py-2 text-sm text-right',
              'transition-colors duration-150',
              item.disabled
                ? 'text-gray-400 cursor-not-allowed'
                : item.danger
                ? 'text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20'
                : 'text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700'
            )}
          >
            {item.icon && (
              <span className="flex-shrink-0 text-gray-400">{item.icon}</span>
            )}
            <span>{item.label}</span>
          </button>
        );
      })}
    </div>,
    document.body
  );
}

// ==================== Main Component ====================

export function Dropdown({
  trigger,
  items,
  onItemClick,
  placement = 'bottom-end',
  minWidth = 160,
  disabled = false,
  className,
}: DropdownProps) {
  const [isOpen, setIsOpen] = useState(false);
  const triggerRef = useRef<HTMLDivElement>(null);

  const handleTriggerClick = useCallback(
    (e: MouseEvent) => {
      e.stopPropagation();
      if (!disabled) {
        setIsOpen((prev) => !prev);
      }
    },
    [disabled]
  );

  const handleItemClick = useCallback(
    (item: DropdownItem) => {
      if (!item.disabled) {
        onItemClick?.(item);
        setIsOpen(false);
      }
    },
    [onItemClick]
  );

  const handleClose = useCallback(() => {
    setIsOpen(false);
  }, []);

  return (
    <div className={clsx('relative inline-block', className)}>
      <div
        ref={triggerRef}
        onClick={handleTriggerClick}
        className={clsx(disabled && 'opacity-50 cursor-not-allowed')}
      >
        {trigger}
      </div>

      <DropdownMenu
        isOpen={isOpen}
        items={items}
        onItemClick={handleItemClick}
        onClose={handleClose}
        triggerRef={triggerRef}
        placement={placement}
        minWidth={minWidth}
      />
    </div>
  );
}

export default Dropdown;

