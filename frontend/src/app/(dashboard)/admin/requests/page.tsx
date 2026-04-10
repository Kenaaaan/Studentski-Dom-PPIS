"use client";

import { useAuth } from '@/context/AuthContext';
import { Card } from '@/components/ui/card';
import { FileText } from 'lucide-react';

export default function AdminRequestsPage() {
  const { user } = useAuth();

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 tracking-tight flex items-center gap-2">
            <FileText className="text-indigo-600" />
            Upravljanje Zahtjevima
          </h1>
          <p className="text-gray-500 mt-1">Pregled svih zahtjeva studenata i ažuriranje statusa obrade</p>
        </div>
      </div>
      
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden flex flex-col items-center justify-center py-24 text-center">
        <FileText size={48} className="text-gray-300 mb-4" />
        <h2 className="text-xl font-medium text-gray-700">Modul u razvoju</h2>
        <p className="text-gray-500 mt-2 max-w-md">Trenutno testirate prototip. Ovaj dio sistema je vezan za API i bit će uskoro potpuno funkcionalan na frontend klijentu.</p>
      </div>
    </div>
  );
}
