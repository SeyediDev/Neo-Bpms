/**
 * Live Operations Types
 * Neo BPMS Admin Panel Core
 */

export type TimeFrame = '1m' | '5m' | '15m' | '1h' | '24h';

export interface TimeFrameOption {
  value: TimeFrame;
  label: string;
  labelEn: string;
  seconds: number;
}

export const TIME_FRAME_OPTIONS: TimeFrameOption[] = [
  { value: '1m', label: '۱ دقیقه', labelEn: '1 min', seconds: 60 },
  { value: '5m', label: '۵ دقیقه', labelEn: '5 min', seconds: 300 },
  { value: '15m', label: '۱۵ دقیقه', labelEn: '15 min', seconds: 900 },
  { value: '1h', label: '۱ ساعت', labelEn: '1 hour', seconds: 3600 },
  { value: '24h', label: '۲۴ ساعت', labelEn: '24 hours', seconds: 86400 },
];

export type TrendDirection = 'up' | 'down' | 'stable';

export interface LiveMetric {
  id: string;
  name: string;
  nameEn?: string;
  value: number;
  previousValue?: number;
  unit?: string;
  format?: 'number' | 'currency' | 'percent';
  trend?: TrendDirection;
  trendPercent?: number;
  sparklineData?: number[];
  color?: 'primary' | 'success' | 'warning' | 'danger' | 'info';
  icon?: string;
  category?: string;
}

export interface LiveOperationsData {
  metrics: LiveMetric[];
  lastUpdated: Date;
  timeFrame: TimeFrame;
}

export interface LiveOperationsFilter {
  timeFrame: TimeFrame;
  categories?: string[];
  refreshInterval?: number;
}








