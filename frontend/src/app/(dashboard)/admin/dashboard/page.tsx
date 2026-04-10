"use client";

import { useAuth } from '@/context/AuthContext';
import { Card } from '@/components/ui/card';

export default function AdminDashboard() {
  const { user } = useAuth();

  return (
    <div className="space-y-6 max-w-6xl mx-auto">
      <div>
        <h1 className="text-2xl font-bold text-gray-900 tracking-tight">Admin & Staff Portal</h1>
        <p className="text-gray-500 mt-1">Dobrodošli, {user?.firstName}. Vaša uloga: <span className="font-semibold text-indigo-600">{user?.role}</span></p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
          <h3 className="font-semibold text-gray-900 mb-2">Novi Zahtjevi</h3>
          <p className="text-3xl font-bold text-indigo-600">12</p>
        </div>
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
          <h3 className="font-semibold text-gray-900 mb-2">Slobodne Sobe</h3>
          <p className="text-3xl font-bold text-green-600">5</p>
        </div>
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
          <h3 className="font-semibold text-gray-900 mb-2">Aktivni Korisnici</h3>
          <p className="text-3xl font-bold text-blue-600">342</p>
        </div>
      </div>
    </div>
  );
}
