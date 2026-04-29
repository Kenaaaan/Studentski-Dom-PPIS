"use client";

import { useEffect, useState } from 'react';
import api from '@/lib/api';
import { useAuth } from '@/context/AuthContext';
import { AccessRight, User, Room, Resource } from '@/types';
import { Shield, Search, Plus, Trash2, Key, X } from 'lucide-react';
import { toast } from 'react-hot-toast';

export default function AdminAccessPage() {
  const [accessRights, setAccessRights] = useState<AccessRight[]>([]);
  const [students, setStudents] = useState<User[]>([]);
  const [rooms, setRooms] = useState<Room[]>([]);
  const [resources, setResources] = useState<Resource[]>([]);
  
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const { user } = useAuth();

  // Form state
  const [selectedUser, setSelectedUser] = useState('');
  const [accessType, setAccessType] = useState('Room');
  const [selectedTarget, setSelectedTarget] = useState('');
  const [reason, setReason] = useState('');
  const [expiresAt, setExpiresAt] = useState('');

  const fetchData = async () => {
    try {
      setLoading(true);
      const [accessRes, studentsRes, roomsRes, resourcesRes] = await Promise.all([
        api.get('/access-rights'),
        api.get('/users/students'),
        api.get('/rooms'),
        api.get('/resources')
      ]);
      setAccessRights(accessRes.data);
      setStudents(studentsRes.data);
      setRooms(roomsRes.data);
      setResources(resourcesRes.data);
    } catch (error) {
      console.error('Failed to fetch data', error);
      toast.error('Greška pri učitavanju podataka');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleGrantAccess = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedUser || !selectedTarget) {
      toast.error('Molimo popunite sva obavezna polja');
      return;
    }

    try {
      const payload = {
        userId: selectedUser,
        accessType,
        roomId: accessType === 'Room' ? selectedTarget : null,
        resourceId: accessType !== 'Room' ? selectedTarget : null,
        reason,
        expiresAt: expiresAt || null
      };

      await api.post('/access-rights', payload);
      toast.success('Pristup uspješno dodijeljen');
      setShowModal(false);
      fetchData();
      
      // Reset form
      setSelectedUser('');
      setSelectedTarget('');
      setReason('');
      setExpiresAt('');
    } catch (error) {
      toast.error('Greška pri dodjeli pristupa');
    }
  };

  const handleRevokeAccess = async (id: string) => {
    if (!confirm('Jeste li sigurni da želite ukinuti ovaj pristup?')) return;
    try {
      await api.put(`/access-rights/${id}/revoke`);
      toast.success('Pristup ukinut');
      fetchData();
    } catch (error) {
      toast.error('Greška pri ukidanju pristupa');
    }
  };

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
        
        <button 
          onClick={() => setShowModal(true)}
          className="inline-flex items-center justify-center px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition font-medium text-sm"
        >
          <Plus size={18} className="mr-2" />
          Dodijeli Novi Pristup
        </button>
      </div>

      {/* Modal */}
      {showModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
          <div className="bg-white rounded-xl shadow-xl max-w-md w-full overflow-hidden">
            <div className="p-6 border-b border-gray-100 flex justify-between items-center">
              <h3 className="text-lg font-bold text-gray-900">Novi Pristup</h3>
              <button onClick={() => setShowModal(false)} className="text-gray-400 hover:text-gray-600">
                <X size={20} />
              </button>
            </div>
            <form onSubmit={handleGrantAccess} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Student</label>
                <select 
                  className="w-full border border-gray-300 rounded-lg p-2 text-sm"
                  value={selectedUser}
                  onChange={(e) => setSelectedUser(e.target.value)}
                  required
                >
                  <option value="">Odaberi studenta...</option>
                  {students.map(s => <option key={s.id} value={s.id}>{s.firstName} {s.lastName} ({s.email})</option>)}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Tip Pristupa</label>
                <select 
                  className="w-full border border-gray-300 rounded-lg p-2 text-sm"
                  value={accessType}
                  onChange={(e) => {
                    setAccessType(e.target.value);
                    setSelectedTarget('');
                  }}
                >
                  <option value="Room">Soba</option>
                  <option value="Building">Zgrada (Ulaz)</option>
                  <option value="CommonArea">Zajednička prostorija</option>
                  <option value="Network">Mreža</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {accessType === 'Room' ? 'Soba' : 'Resurs'}
                </label>
                <select 
                  className="w-full border border-gray-300 rounded-lg p-2 text-sm"
                  value={selectedTarget}
                  onChange={(e) => setSelectedTarget(e.target.value)}
                  required
                >
                  <option value="">Odaberi...</option>
                  {accessType === 'Room' ? (
                    rooms.map(r => <option key={r.id} value={r.id}>Soba {r.roomNumber} ({r.building})</option>)
                  ) : (
                    resources.filter(r => 
                      (accessType === 'CommonArea' && r.resourceType === 'CommonArea') ||
                      (accessType === 'Building' && (r.resourceType === 'Other' || r.name.toLowerCase().includes('ulaz'))) ||
                      (accessType === 'Network' && r.resourceType === 'Network') ||
                      (accessType === 'Other')
                    ).map(r => <option key={r.id} value={r.id}>{r.name}</option>)
                  )}
                  {/* Ako nema filtriranih resursa, pokaži sve */}
                  {accessType !== 'Room' && resources.length > 0 && 
                    <option disabled>--- Svi resursi ---</option> &&
                    resources.map(r => <option key={r.id} value={r.id}>{r.name}</option>)
                  }
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Razlog (opcionalno)</label>
                <input 
                  type="text"
                  className="w-full border border-gray-300 rounded-lg p-2 text-sm"
                  value={reason}
                  onChange={(e) => setReason(e.target.value)}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Ističe (opcionalno)</label>
                <input 
                  type="date"
                  className="w-full border border-gray-300 rounded-lg p-2 text-sm"
                  value={expiresAt}
                  onChange={(e) => setExpiresAt(e.target.value)}
                />
              </div>

              <div className="pt-4 flex gap-3">
                <button 
                  type="button"
                  onClick={() => setShowModal(false)}
                  className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition text-sm font-medium"
                >
                  Otkaži
                </button>
                <button 
                  type="submit"
                  className="flex-1 px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition text-sm font-medium"
                >
                  Dodijeli
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

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
                        {access.isActive && (
                          <button 
                            onClick={() => handleRevokeAccess(access.id)}
                            className="text-red-600 hover:text-red-800 bg-red-50 hover:bg-red-100 p-2 rounded-lg transition" 
                            title="Ukini pristup"
                          >
                            <Trash2 size={16} />
                          </button>
                        )}
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
