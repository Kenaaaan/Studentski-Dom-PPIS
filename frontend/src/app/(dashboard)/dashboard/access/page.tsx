"use client";

import { useEffect, useState } from 'react';
import api from '@/lib/api';
import { useAuth } from '@/context/AuthContext';
import { AccessRight } from '@/types';
import { Key, Building2, Wifi, Shield, Info, HelpCircle } from 'lucide-react';

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
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 tracking-tight">Moji Pristupi</h1>
          <p className="text-gray-500 mt-1">Pregled vaših digitalnih ključeva i dozvola za resurse doma</p>
        </div>
        <div className="bg-indigo-50 border border-indigo-100 rounded-lg p-3 flex items-start gap-3 max-w-md">
          <Info className="text-indigo-600 mt-0.5 shrink-0" size={18} />
          <p className="text-xs text-indigo-800">
            Ovdje se nalaze vaša prava pristupa za sobu, zajedničke prostorije i mrežu. 
            Ovi pristupi vam omogućavaju korištenje pametnih brava i prijavu na servise doma.
          </p>
        </div>
      </div>

      {loading ? (
        <div className="animate-pulse space-y-4">
          {[1, 2, 3].map(i => (
            <div key={i} className="h-24 bg-gray-200 rounded-xl"></div>
          ))}
        </div>
      ) : accessRights.length === 0 ? (
        <div className="bg-white p-12 rounded-xl border border-gray-100 text-center shadow-sm">
          <div className="bg-gray-50 w-20 h-20 rounded-full flex items-center justify-center mx-auto mb-6">
            <Key className="h-10 w-10 text-gray-300" />
          </div>
          <h3 className="text-xl font-bold text-gray-900">Nemate aktivnih pristupa</h3>
          <p className="text-gray-500 mt-2 max-w-sm mx-auto">
            Trenutno nemate dodijeljenih prava pristupa u sistemu. Ako ste se tek uselili, 
            molimo sačekajte da administracija obradi vaše podatke ili ih kontaktirajte.
          </p>
          <div className="mt-8 flex justify-center gap-4">
            <div className="flex items-center gap-2 text-sm text-gray-600 bg-gray-50 px-4 py-2 rounded-full">
              <HelpCircle size={16} />
              <span>Trebate pomoć? Posjetite recepciju.</span>
            </div>
          </div>
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
                  <div className="flex flex-col items-end gap-1">
                    <span className={`px-2.5 py-1 text-xs font-semibold rounded-full ${access.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                      {access.isActive ? 'Aktivno' : 'Neaktivno'}
                    </span>
                    {access.id === '00000000-0000-0000-0000-000000000000' && (
                      <span className="px-2 py-0.5 text-[10px] font-bold bg-blue-50 text-blue-600 rounded border border-blue-100 uppercase">
                        Zadani pristup
                      </span>
                    )}
                  </div>
                </div>
                
                <h3 className="text-lg font-bold text-gray-900 mb-1">
                  {access.accessType === 'Room' ? `Soba ${access.roomNumber}` : access.resourceName || 'Resurs'}
                </h3>
                
                <p className="text-sm text-gray-500 mb-4 h-10 overflow-hidden text-ellipsis">
                  {access.reason || 'Nema opisa'}
                </p>

                <div className="space-y-2 text-sm text-gray-600 border-t border-gray-50 pt-4 mt-auto">
                  <div className="flex justify-between">
                    <span>Odobrio:</span>
                    <span className="font-medium text-gray-900">{access.grantedByName || 'Sistem'}</span>
                  </div>
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
