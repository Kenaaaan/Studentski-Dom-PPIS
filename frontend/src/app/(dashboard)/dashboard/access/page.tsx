"use client";

import { useEffect, useState } from 'react';
import api from '@/lib/api';
import { useAuth } from '@/context/AuthContext';
import { AccessRight } from '@/types';
import { Key, Building2, Wifi, Shield } from 'lucide-react';

export default function AccessRightsPage() {
  const [accessRights, setAccessRights] = useState<AccessRight[]>([]);
  const [loading, setLoading] = useState(true);
  const { user } = useAuth();

  useEffect(() => {
    const fetchAccessRights = async () => {
      if (!user) return;
      try {
        const response = await api.get(`/access-rights/user/${user.id}`);
        setAccessRights(response.data);
      } catch (error) {
        console.error('Failed to fetch access rights', error);
      } finally {
        setLoading(false);
      }
    };
    fetchAccessRights();
  }, [user]);

  const getIcon = (type: string) => {
    switch (type) {
      case 'Room': return <Building2 className="text-indigo-500" />;
      case 'Network': return <Wifi className="text-blue-500" />;
      case 'Building': return <Shield className="text-emerald-500" />;
      default: return <Key className="text-amber-500" />;
    }
  };

  return (
    <div className="max-w-6xl mx-auto space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 tracking-tight">Moji Pristupi</h1>
          <p className="text-gray-500 mt-1">Pregled svih odobrenih pristupa resursima doma</p>
        </div>
      </div>

      {loading ? (
        <div className="animate-pulse space-y-4">
          {[1, 2, 3].map(i => (
            <div key={i} className="h-24 bg-gray-200 rounded-xl"></div>
          ))}
        </div>
      ) : accessRights.length === 0 ? (
        <div className="bg-white p-8 rounded-xl border border-gray-100 text-center">
          <Key className="mx-auto h-12 w-12 text-gray-300 mb-4" />
          <h3 className="text-lg font-medium text-gray-900">Nemate aktivnih pristupa</h3>
          <p className="text-gray-500 mt-2">Trenutno nemate dodijeljenih prava pristupa u sistemu.</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {accessRights.map((access) => (
            <div key={access.id} className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden hover:shadow-md transition-shadow">
              <div className="p-6">
                <div className="flex justify-between items-start mb-4">
                  <div className="p-3 bg-gray-50 rounded-lg">
                    {getIcon(access.accessType)}
                  </div>
                  <span className={`px-2.5 py-1 text-xs font-semibold rounded-full ${access.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                    {access.isActive ? 'Aktivno' : 'Neaktivno'}
                  </span>
                </div>
                
                <h3 className="text-lg font-bold text-gray-900 mb-1">
                  {access.accessType === 'Room' ? `Soba ${access.roomNumber}` : access.resourceName || 'Resurs'}
                </h3>
                
                <p className="text-sm text-gray-500 mb-4 h-10 overflow-hidden text-ellipsis">
                  {access.reason || 'Nema opisa'}
                </p>

                <div className="space-y-2 text-sm text-gray-600 border-t border-gray-50 pt-4 mt-auto">
                  <div className="flex justify-between">
                    <span>Dodijeljeno:</span>
                    <span className="font-medium text-gray-900">{new Date(access.grantedAt).toLocaleDateString('bs-BA')}</span>
                  </div>
                  {access.expiresAt && (
                    <div className="flex justify-between">
                      <span>Ističe:</span>
                      <span className="font-medium text-gray-900">{new Date(access.expiresAt).toLocaleDateString('bs-BA')}</span>
                    </div>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
