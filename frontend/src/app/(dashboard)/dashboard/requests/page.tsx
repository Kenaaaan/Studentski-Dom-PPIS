"use client";

import { useEffect, useState } from 'react';
import api from '@/lib/api';
import { useAuth } from '@/context/AuthContext';
import { Request } from '@/types';
import { FileText, Plus, Clock, CheckCircle2, XCircle, AlertCircle } from 'lucide-react';

export default function RequestsPage() {
  const [requests, setRequests] = useState<Request[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const { user } = useAuth();
  
  // New request form state
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [requestType, setRequestType] = useState('Maintenance');

  const fetchRequests = async () => {
    try {
      const response = await api.get('/requests/my');
      setRequests(response.data);
    } catch (error) {
      console.error('Failed to fetch requests', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRequests();
  }, []);

  const handleCreateRequest = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await api.post('/requests', {
        title,
        description,
        requestType,
        priority: 'Medium', // Default for students
      });
      setShowModal(false);
      fetchRequests(); // Refresh the list
      // Reset form
      setTitle('');
      setDescription('');
      setRequestType('Maintenance');
    } catch (error) {
      alert('Greška prilikom podnošenja zahtjeva.');
    }
  };

  const getStatusBadge = (status: string) => {
    switch (status) {
      case 'Pending': return <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-gray-100 text-gray-800"><Clock size={14}/> Na čekanju</span>;
      case 'InProgress': return <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-blue-100 text-blue-800"><AlertCircle size={14}/> U obradi</span>;
      case 'Resolved': return <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-800"><CheckCircle2 size={14}/> Riješeno</span>;
      case 'Rejected': return <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-red-100 text-red-800"><XCircle size={14}/> Odbijeno</span>;
      default: return null;
    }
  };

  return (
    <div className="max-w-6xl mx-auto space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 tracking-tight">Moji Zahtjevi</h1>
          <p className="text-gray-500 mt-1">Pregled i podnošenje zahtjeva za administraciju i osoblje</p>
        </div>
        <button
          onClick={() => setShowModal(true)}
          className="inline-flex items-center justify-center px-4 py-2 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 transition-colors"
        >
          <Plus size={18} className="mr-2" />
          Novi Zahtjev
        </button>
      </div>

      {loading ? (
        <div className="animate-pulse space-y-4">
          {[1, 2].map(i => <div key={i} className="h-32 bg-white rounded-xl border border-gray-100"></div>)}
        </div>
      ) : requests.length === 0 ? (
        <div className="bg-white p-12 rounded-xl border border-gray-100 text-center">
          <FileText className="mx-auto h-12 w-12 text-gray-300 mb-4" />
          <h3 className="text-lg font-medium text-gray-900">Nemate podnesenih zahtjeva</h3>
          <p className="text-gray-500 mt-2">Trenutno nemate otvorenih ili prošlih zahtjeva u sistemu.</p>
        </div>
      ) : (
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
          <ul className="divide-y divide-gray-200">
            {requests.map(req => (
              <li key={req.id} className="p-6 hover:bg-gray-50 transition-colors">
                <div className="flex flex-col sm:flex-row sm:justify-between sm:items-start gap-4">
                  <div className="flex-1">
                    <div className="flex items-center gap-3 mb-1">
                      <h3 className="text-lg font-bold text-gray-900">{req.title}</h3>
                      {getStatusBadge(req.status)}
                    </div>
                    <p className="text-sm text-gray-600 mt-2 max-w-2xl">{req.description}</p>
                    <div className="mt-4 flex flex-wrap gap-4 text-xs text-gray-500">
                      <span>Tip: <strong className="font-medium text-gray-700">{req.requestType}</strong></span>
                      <span>Podneseno: {new Date(req.createdAt).toLocaleString('bs-BA')}</span>
                      {req.assigneeName && <span>Zadužen: {req.assigneeName}</span>}
                    </div>
                  </div>
                  <button className="text-sm font-medium text-indigo-600 hover:text-indigo-800 whitespace-nowrap self-start">
                    Vidi detalje →
                  </button>
                </div>
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Create Request Modal */}
      {showModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-gray-900/50 backdrop-blur-sm">
          <div className="bg-white rounded-2xl shadow-xl max-w-md w-full overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center">
              <h3 className="text-lg font-bold text-gray-900">Podnesi Novi Zahtjev</h3>
              <button onClick={() => setShowModal(false)} className="text-gray-400 hover:text-gray-500">
                <XCircle size={24} />
              </button>
            </div>
            <div className="p-6">
              <form onSubmit={handleCreateRequest} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Tip Zahtjeva</label>
                  <select 
                    value={requestType} 
                    onChange={e => setRequestType(e.target.value)}
                    className="w-full border border-gray-300 rounded-lg p-2.5 focus:ring-indigo-500 focus:border-indigo-500"
                  >
                    <option value="Maintenance">Tehničko Održavanje (Kvar)</option>
                    <option value="InventoryReplacement">Zamjena Inventara</option>
                    <option value="ResidenceCertificate">Potvrda o Boravku</option>
                    <option value="Complaint">Žalba</option>
                    <option value="Other">Ostalo</option>
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Naslov</label>
                  <input 
                    type="text" 
                    required 
                    value={title} 
                    onChange={e => setTitle(e.target.value)}
                    className="w-full border border-gray-300 rounded-lg p-2.5 focus:ring-indigo-500 focus:border-indigo-500"
                    placeholder="Kratak opis problema"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Detaljan opis</label>
                  <textarea 
                    required 
                    value={description} 
                    onChange={e => setDescription(e.target.value)}
                    rows={4}
                    className="w-full border border-gray-300 rounded-lg p-2.5 focus:ring-indigo-500 focus:border-indigo-500"
                    placeholder="Opišite detaljno šta vam je potrebno..."
                  ></textarea>
                </div>
                <div className="pt-4 flex gap-3">
                  <button 
                    type="button" 
                    onClick={() => setShowModal(false)}
                    className="flex-1 py-2.5 border border-gray-300 text-gray-700 rounded-lg font-medium hover:bg-gray-50"
                  >
                    Odustani
                  </button>
                  <button 
                    type="submit"
                    className="flex-1 py-2.5 bg-indigo-600 text-white rounded-lg font-medium hover:bg-indigo-700"
                  >
                    Pošalji Zahtjev
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
