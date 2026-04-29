"use client";

import { useEffect, useState } from 'react';
import api from '@/lib/api';
import { Room, Resource } from '@/types';
import { Building2, Home, Package, CheckCircle, XCircle } from 'lucide-react';
import { toast } from 'react-hot-toast';

export default function AdminResourcesPage() {
  const [rooms, setRooms] = useState<Room[]>([]);
  const [resources, setResources] = useState<Resource[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'rooms' | 'resources'>('rooms');
  const [buildingFilter, setBuildingFilter] = useState('All');

  const fetchData = async () => {
    try {
      setLoading(true);
      const [roomsRes, resourcesRes] = await Promise.all([
        api.get('/rooms'),
        api.get('/resources')
      ]);
      setRooms(roomsRes.data);
      setResources(resourcesRes.data);
    } catch (error) {
      console.error('Failed to fetch resources', error);
      toast.error('Greška pri učitavanju podataka');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const toggleRoomAvailability = async (room: Room) => {
    try {
      await api.put(`/rooms/${room.id}`, { isAvailable: !room.isAvailable });
      toast.success(`Soba ${room.roomNumber} je sada ${!room.isAvailable ? 'raspoloživa' : 'nedostupna'}`);
      setRooms(rooms.map(r => r.id === room.id ? { ...r, isAvailable: !r.isAvailable } : r));
    } catch (error) {
      toast.error('Greška pri ažuriranju sobe');
    }
  };

  const toggleResourceStatus = async (resource: Resource) => {
    try {
      await api.put(`/resources/${resource.id}`, { isActive: !resource.isActive });
      toast.success(`Resurs ${resource.name} je sada ${!resource.isActive ? 'aktivan' : 'neaktivan'}`);
      setResources(resources.map(r => r.id === resource.id ? { ...r, isActive: !r.isActive } : r));
    } catch (error) {
      toast.error('Greška pri ažuriranju resursa');
    }
  };

  const setTemporaryUnavailable = async (resource: Resource) => {
    try {
      const until = new Date();
      until.setHours(until.getHours() + 24); // Označavamo na 24h
      await api.put(`/resources/${resource.id}`, { unavailableUntil: until.toISOString() });
      toast.success(`Resurs ${resource.name} je označen kao nedostupan naredna 24h`);
      fetchData();
    } catch (error) {
      toast.error('Greška pri postavljanju nedostupnosti');
    }
  };

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 tracking-tight flex items-center gap-2">
            <Building2 className="text-indigo-600" />
            Sobe i Resursi
          </h1>
          <p className="text-gray-500 mt-1">Konfiguracija i upravljanje raspoloživošću smještaja i zajedničkih resursa</p>
        </div>
      </div>

      <div className="flex border-b border-gray-200">
        <button
          onClick={() => setActiveTab('rooms')}
          className={`px-6 py-3 text-sm font-medium transition-colors relative ${
            activeTab === 'rooms' ? 'text-indigo-600' : 'text-gray-500 hover:text-gray-700'
          }`}
        >
          <div className="flex items-center gap-2">
            <Home size={18} />
            Sobe
          </div>
          {activeTab === 'rooms' && <div className="absolute bottom-0 left-0 right-0 h-0.5 bg-indigo-600" />}
        </button>
        <button
          onClick={() => setActiveTab('resources')}
          className={`px-6 py-3 text-sm font-medium transition-colors relative ${
            activeTab === 'resources' ? 'text-indigo-600' : 'text-gray-500 hover:text-gray-700'
          }`}
        >
          <div className="flex items-center gap-2">
            <Package size={18} />
            Zajednički Resursi
          </div>
          {activeTab === 'resources' && <div className="absolute bottom-0 left-0 right-0 h-0.5 bg-indigo-600" />}
        </button>
      </div>

      {loading ? (
        <div className="p-12 text-center text-gray-500 animate-pulse bg-white rounded-xl border border-gray-200">
          Učitavanje podataka...
        </div>
      ) : (
        <div className="space-y-4">
          {activeTab === 'rooms' && (
            <div className="flex items-center gap-2 bg-white p-4 rounded-xl border border-gray-200 shadow-sm">
              <span className="text-sm font-medium text-gray-700">Filtriraj po paviljonu:</span>
              <select 
                className="text-sm border border-gray-300 rounded-lg p-1.5 focus:ring-indigo-500 focus:border-indigo-500"
                value={buildingFilter}
                onChange={(e) => setBuildingFilter(e.target.value)}
              >
                <option value="All">Svi paviljoni</option>
                {Array.from(new Set(rooms.map(r => r.building))).map(b => (
                  <option key={b} value={b}>{b}</option>
                ))}
              </select>
            </div>
          )}

          <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
            {activeTab === 'rooms' ? (
              <div className="overflow-x-auto">
                <table className="w-full text-sm text-left text-gray-500">
                  <thead className="text-xs text-gray-700 uppercase bg-gray-50 border-b border-gray-200">
                    <tr>
                      <th className="px-6 py-4">Soba</th>
                      <th className="px-6 py-4">Zgrada / Sprat</th>
                      <th className="px-6 py-4">Tip</th>
                      <th className="px-6 py-4">Kapacitet</th>
                      <th className="px-6 py-4">Status</th>
                      <th className="px-6 py-4 text-right">Akcije</th>
                    </tr>
                  </thead>
                  <tbody>
                    {rooms
                      .filter(r => buildingFilter === 'All' || r.building === buildingFilter)
                      .map((room) => (
                    <tr key={room.id} className="bg-white border-b border-gray-100 hover:bg-gray-50 transition">
                      <td className="px-6 py-4 font-bold text-gray-900">
                        {room.roomNumber}
                      </td>
                      <td className="px-6 py-4">
                        {room.building}, {room.floor}. sprat
                      </td>
                      <td className="px-6 py-4">
                        {room.roomType}
                      </td>
                      <td className="px-6 py-4">
                        {room.capacity} kreveta
                      </td>
                      <td className="px-6 py-4">
                        <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                          room.isAvailable ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                        }`}>
                          {room.isAvailable ? 'Slobodna' : 'Zauzeta/Nedostupna'}
                        </span>
                      </td>
                      <td className="px-6 py-4 text-right">
                        <button
                          onClick={() => toggleRoomAvailability(room)}
                          className={`p-2 rounded-lg transition ${
                            room.isAvailable ? 'text-amber-600 bg-amber-50 hover:bg-amber-100' : 'text-green-600 bg-green-50 hover:bg-green-100'
                          }`}
                          title={room.isAvailable ? "Označi kao nedostupnu" : "Označi kao dostupnu"}
                        >
                          {room.isAvailable ? <XCircle size={18} /> : <CheckCircle size={18} />}
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-sm text-left text-gray-500">
                <thead className="text-xs text-gray-700 uppercase bg-gray-50 border-b border-gray-200">
                  <tr>
                    <th className="px-6 py-4">Resurs</th>
                    <th className="px-6 py-4">Tip</th>
                    <th className="px-6 py-4">Lokacija</th>
                    <th className="px-6 py-4">Status</th>
                    <th className="px-6 py-4 text-right">Akcije</th>
                  </tr>
                </thead>
                <tbody>
                  {resources.map((res) => (
                    <tr key={res.id} className="bg-white border-b border-gray-100 hover:bg-gray-50 transition">
                      <td className="px-6 py-4">
                        <div className="font-medium text-gray-900">{res.name}</div>
                        <div className="text-xs text-gray-400">{res.description}</div>
                      </td>
                      <td className="px-6 py-4">
                        {res.resourceType}
                      </td>
                      <td className="px-6 py-4">
                        {res.location || '-'}
                      </td>
                      <td className="px-6 py-4">
                        <div className="flex flex-col">
                          <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                            res.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                          }`}>
                            {res.isActive ? 'Aktivan' : 'Neaktivan/Nedostupan'}
                          </span>
                          {res.unavailableUntil && new Date(res.unavailableUntil) > new Date() && (
                            <span className="text-[10px] text-amber-600 font-medium mt-1">
                              Do: {new Date(res.unavailableUntil).toLocaleString()}
                            </span>
                          )}
                        </div>
                      </td>
                      <td className="px-6 py-4 text-right flex justify-end gap-2">
                        <button
                          onClick={() => toggleResourceStatus(res)}
                          className={`p-2 rounded-lg transition ${
                            res.isActive ? 'text-amber-600 bg-amber-50 hover:bg-amber-100' : 'text-green-600 bg-green-50 hover:bg-green-100'
                          }`}
                          title={res.isActive ? "Deaktiviraj" : "Aktiviraj"}
                        >
                          {res.isActive ? <XCircle size={18} /> : <CheckCircle size={18} />}
                        </button>
                        <button
                          onClick={() => setTemporaryUnavailable(res)}
                          className="p-2 rounded-lg text-red-600 bg-red-50 hover:bg-red-100 transition"
                          title="Označi kao nedostupno na 24h"
                        >
                          <XCircle size={18} />
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    )}
  </div>
);
}
