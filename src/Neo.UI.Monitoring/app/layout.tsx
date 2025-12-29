import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  title: 'مانیتورینگ سیستم | Neo BPMS',
  description: 'داشبورد مانیتورینگ سیستم Neo BPMS',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="fa" dir="rtl" className="dark">
      <body className="min-h-screen bg-slate-950 text-white antialiased">
        {children}
      </body>
    </html>
  );
}

