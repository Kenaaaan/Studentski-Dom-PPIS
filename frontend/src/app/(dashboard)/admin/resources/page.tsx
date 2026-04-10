"use client";

import { useAuth } from '@/context/AuthContext';
import { Building2 } from 'lucide-react';

export default function AdminResourcesPage() {
  const { user } = useAuth();

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 tracking-tight flex items-center gap-2">
            <Building2 className="text-indigo-600" />
            Sobe i Resursi
          </h1>
          <p className="text-gray-500 mt-1">Konfiguracija i uvid u popunjenost soba te upravljanje inventarom</p>
        </div>
      </div>
      
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden flex flex-col items-center justify-center py-24 text-center">
        <Building2 size={48} className="text-gray-300 mb-4" />
        <h2 className="text-xl font-medium text-gray-700">Modul u razvoju</h2>
        <p className="text-gray-500 mt-2 max-w-md">Ovdje će se nalaziti interaktivna lista paviljona, soba te zajedničkih prostorija za konfiguraciju raspoloživosti.</p>
      </div>
    </div>
  );
}
