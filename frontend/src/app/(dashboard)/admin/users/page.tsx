"use client";

import { useAuth } from '@/context/AuthContext';
import { Users } from 'lucide-react';

export default function AdminUsersPage() {
  const { user } = useAuth();

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 tracking-tight flex items-center gap-2">
            <Users className="text-indigo-600" />
            Upravljanje Korisnicima
          </h1>
          <p className="text-gray-500 mt-1">Pregled administratora, osoblja i evidencija svih studenata</p>
        </div>
      </div>
      
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden flex flex-col items-center justify-center py-24 text-center">
        <Users size={48} className="text-gray-300 mb-4" />
        <h2 className="text-xl font-medium text-gray-700">Modul u razvoju</h2>
        <p className="text-gray-500 mt-2 max-w-md">Uskoro ćete ovdje moći brisati ili deaktivirati korisnike te im mijenjati korisničke role.</p>
      </div>
    </div>
  );
}
