"use client";

import { useEffect, useState } from 'react';
import api from '@/lib/api';
import { useAuth } from '@/context/AuthContext';
import { AccessRight, User } from '@/types';
import { Shield, Search, Plus, Trash2, Key } from 'lucide-react';

export default function AdminAccessPage() {
  const [accessRights, setAccessRights] = useState<AccessRight[]>([]);
  const [loading, setLoading] = useState(true);
  const { user } = useAuth();

  useEffect(() => {
    const fetchAllAccessRights = async () => {
      try {
        const response = await api.get('/access-rights');
        setAccessRights(response.data);
      } catch (error) {
        console.error('Failed to fetch access rights', error);
      } finally {
        setLoading(false);
      }
    };
    fetchAllAccessRights();
  }, []);

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 tracking-tight flex items-center gap-2">
            <Shield className="text-indigo-600" />
            Upravljanje Pristupima
          </h1>
          <p className="text-gray-500 mt-1">Dodjela i pregled prava pristupa sobama i resursima za studente</p>
        </div>
        
        <button className="inline-flex items-center justify-center px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition font-medium text-sm">
          <Plus size={18} className="mr-2" />
          Dodijeli Novi Pristup
        </button>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
        <div className="p-4 border-b border-gray-200 bg-gray-50 flex items-center gap-4">
          <div className="relative flex-1 max-w-md">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-gray-400">
              <Search size={18} />
            </div>
            <input
              type="text"
              placeholder="Pretraži po studentu ili resursu..."
              className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500 text-sm"
            />
          </div>
        </div>

        {loading ? (
          <div className="p-8 text-center text-gray-500 animate-pulse">Učitavanje podataka...</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm text-left text-gray-500">
              <thead className="text-xs text-gray-700 uppercase bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-6 py-4">Student</th>
                  <th className="px-6 py-4">Tip Pristupa</th>
                  <th className="px-6 py-4">Resurs / Soba</th>
                  <th className="px-6 py-4">Status</th>
                  <th className="px-6 py-4">Kopirano / Ističe</th>
                  <th className="px-6 py-4 text-right">Akcije</th>
                </tr>
              </thead>
              <tbody>
                {accessRights.length > 0 ? (
                  accessRights.map((access) => (
                    <tr key={access.id} className="bg-white border-b border-gray-100 hover:bg-gray-50 transition">
                      <td className="px-6 py-4 font-medium text-gray-900">
                        {access.userName || 'Nepoznat Student'}
                      </td>
                      <td className="px-6 py-4">
                        <span className="inline-flex items-center px-2 py-1 rounded bg-gray-100 text-gray-700 text-xs font-medium">
                          {access.accessType}
                        </span>
                      </td>
                      <td className="px-6 py-4 font-medium text-gray-800">
                        {access.accessType === 'Room' ? `Soba ${access.roomNumber}` : access.resourceName}
                      </td>
                      <td className="px-6 py-4">
                        <span className={`px-2.5 py-1 text-xs font-semibold rounded-full ${access.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                          {access.isActive ? 'Aktivno' : 'Isteklo'}
                        </span>
                      </td>
                      <td className="px-6 py-4 text-xs text-gray-500">
                        <div>Od: {new Date(access.grantedAt).toLocaleDateString('bs-BA')}</div>
                        {access.expiresAt && <div className="text-red-500 mt-0.5">Do: {new Date(access.expiresAt).toLocaleDateString('bs-BA')}</div>}
                      </td>
                      <td className="px-6 py-4 text-right">
                        <button className="text-red-600 hover:text-red-800 bg-red-50 hover:bg-red-100 p-2 rounded-lg transition" title="Ukini pristup">
                          <Trash2 size={16} />
                        </button>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan={6} className="px-6 py-12 text-center text-gray-500">
                      <Key className="mx-auto h-12 w-12 text-gray-300 mb-3" />
                      Nema dodijeljenih prava pristupa u sistemu.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
