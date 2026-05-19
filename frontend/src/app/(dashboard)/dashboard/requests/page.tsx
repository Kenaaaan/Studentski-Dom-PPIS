"use client";

import { useEffect, useState } from 'react';
import api from '@/lib/api';
import { useAuth } from '@/context/AuthContext';
import { Request, Resource, AccessRight } from '@/types';
import { FileText, Plus, Clock, CheckCircle2, XCircle, AlertCircle, X } from 'lucide-react';

const REQUEST_TYPES: { value: string; label: string; needsResource?: boolean }[] = [
  // Pristup
  { value: 'AccessRequest',       label: 'Zahtjev za pristup resursu',       needsResource: true },
  { value: 'LaundryBooking',      label: 'Rezervacija perionice' },
  { value: 'ParkingPermit',       label: 'Dozvola za parking' },
  { value: 'StorageRequest',      label: 'Zahtjev za ostavu' },
  // Soba
  { value: 'RoomChange',          label: 'Zamjena sobe' },
  { value: 'RoomRepair',          label: 'Popravka u sobi' },
  { value: 'KeyReplacement',      label: 'Zamjena ključa / pristupne kartice' },
  // Tehničko
  { value: 'Maintenance',         label: 'Tehničko održavanje (kvar)' },
  { value: 'InternetSupport',     label: 'Problem s internetom' },
  { value: 'InventoryReplacement',label: 'Zamjena inventara' },
  { value: 'CleaningService',     label: 'Usluga čišćenja' },
  // Administrativno
  { value: 'ResidenceCertificate',label: 'Potvrda o boravku' },
  { value: 'GuestRegistration',   label: 'Registracija gosta' },
  { value: 'ComplaintReport',     label: 'Prijava problema / žalba' },
  { value: 'Other',               label: 'Ostalo' },
];

const TYPE_LABELS: Record<string, string> = Object.fromEntries(
  REQUEST_TYPES.map(t => [t.value, t.label])
);

export default function RequestsPage() {
  const { user } = useAuth();
  const [requests, setRequests] = useState<Request[]>([]);
  const [resources, setResources] = useState<Resource[]>([]);
  const [accessRights, setAccessRights] = useState<AccessRight[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState('');

  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [requestType, setRequestType] = useState('Maintenance');
  const [selectedResourceId, setSelectedResourceId] = useState('');

  const fetchData = async () => {
    try {
      const [reqRes, resRes, arRes] = await Promise.all([
        api.get('/requests/my'),
        api.get('/resources'),
        user?.id ? api.get(`/access-rights/user/${user.id}`) : Promise.resolve({ data: [] }),
      ]);
      setRequests(reqRes.data);
      setResources(resRes.data.filter((r: Resource) => r.isActive));
      setAccessRights((arRes.data as AccessRight[]).filter(ar => ar.isActive));
    } catch (error) {
      console.error('Failed to fetch data', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchData(); }, [user?.id]);

  // Resources that the student can still request (no active access right AND no pending/in-progress request)
  const pendingResourceIds = new Set(
    requests
      .filter(r => r.requestType === 'AccessRequest' && (r.status === 'Pending' || r.status === 'InProgress') && r.resourceId)
      .map(r => r.resourceId!)
  );
  const grantedResourceIds = new Set(
    accessRights.filter(ar => ar.resourceId).map(ar => ar.resourceId!)
  );
  const availableResources = resources.filter(
    r => !grantedResourceIds.has(r.id) && !pendingResourceIds.has(r.id)
  );

  const handleTypeChange = (val: string) => {
    setRequestType(val);
    setSelectedResourceId('');
    setSubmitError('');
  };

  const resetModal = () => {
    setShowModal(false);
    setTitle('');
    setDescription('');
    setRequestType('Maintenance');
    setSelectedResourceId('');
    setSubmitError('');
  };

  const handleCreateRequest = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitError('');

    const needsResource = REQUEST_TYPES.find(t => t.value === requestType)?.needsResource;
    if (needsResource && !selectedResourceId) {
      setSubmitError('Odaberite resurs za koji tražite pristup.');
      return;
    }

    setSubmitting(true);
    try {
      await api.post('/requests', {
        title,
        description,
        requestType,
        resourceId: needsResource ? selectedResourceId || null : null,
        priority: 'Medium',
      });
      resetModal();
      fetchData();
    } catch (error: any) {
      const msg = error?.response?.data?.error || 'Greška prilikom podnošenja zahtjeva.';
      setSubmitError(msg);
    } finally {
      setSubmitting(false);
    }
  };

  const getStatusBadge = (status: string) => {
    switch (status) {
      case 'Pending':    return <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-amber-100 text-amber-800"><Clock size={13}/> Na čekanju</span>;
      case 'InProgress': return <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-blue-100 text-blue-800"><AlertCircle size={13}/> U obradi</span>;
      case 'Resolved':   return <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-800"><CheckCircle2 size={13}/> Riješeno</span>;
      case 'Rejected':   return <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-red-100 text-red-800"><XCircle size={13}/> Odbijeno</span>;
      default:           return null;
    }
  };

  const currentTypeNeedsResource = REQUEST_TYPES.find(t => t.value === requestType)?.needsResource;

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
                      <span>Tip: <strong className="font-medium text-gray-700">{TYPE_LABELS[req.requestType] || req.requestType}</strong></span>
                      <span>Podneseno: {new Date(req.createdAt).toLocaleString('bs-BA')}</span>
                      {req.assignedToName && <span>Zadužen: <strong className="font-medium text-gray-700">{req.assignedToName}</strong></span>}
                      {req.resourceName && <span>Resurs: <strong className="font-medium text-gray-700">{req.resourceName}</strong></span>}
                    </div>
                  </div>
                </div>
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Create Request Modal */}
      {showModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-gray-900/50 backdrop-blur-sm">
          <div className="bg-white rounded-2xl shadow-xl max-w-md w-full overflow-hidden max-h-[90vh] flex flex-col">
            <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center flex-shrink-0">
              <h3 className="text-lg font-bold text-gray-900">Podnesi Novi Zahtjev</h3>
              <button onClick={resetModal} className="text-gray-400 hover:text-gray-500">
                <X size={24} />
              </button>
            </div>
            <div className="p-6 overflow-y-auto">
              <form onSubmit={handleCreateRequest} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Tip Zahtjeva</label>
                  <select
                    value={requestType}
                    onChange={e => handleTypeChange(e.target.value)}
                    className="w-full border border-gray-300 rounded-lg p-2.5 focus:ring-indigo-500 focus:border-indigo-500 text-sm"
                  >
                    {REQUEST_TYPES.map(t => (
                      <option key={t.value} value={t.value}>{t.label}</option>
                    ))}
                  </select>
                </div>

                {currentTypeNeedsResource && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Odaberite Resurs</label>
                    {availableResources.length === 0 ? (
                      <p className="text-sm text-amber-600 bg-amber-50 border border-amber-200 rounded-lg px-3 py-2">
                        Već imate pristup svim dostupnim resursima ili imate aktivan zahtjev za njih.
                      </p>
                    ) : (
                      <select
                        value={selectedResourceId}
                        onChange={e => setSelectedResourceId(e.target.value)}
                        className="w-full border border-gray-300 rounded-lg p-2.5 focus:ring-indigo-500 focus:border-indigo-500 text-sm"
                        required
                      >
                        <option value="">Odaberi resurs...</option>
                        {availableResources.map(r => (
                          <option key={r.id} value={r.id}>{r.name} — {r.location}</option>
                        ))}
                      </select>
                    )}
                  </div>
                )}

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Naslov</label>
                  <input
                    type="text"
                    required
                    value={title}
                    onChange={e => setTitle(e.target.value)}
                    className="w-full border border-gray-300 rounded-lg p-2.5 focus:ring-indigo-500 focus:border-indigo-500 text-sm"
                    placeholder="Kratak opis problema ili zahtjeva"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Detaljan opis</label>
                  <textarea
                    required
                    value={description}
                    onChange={e => setDescription(e.target.value)}
                    rows={4}
                    className="w-full border border-gray-300 rounded-lg p-2.5 focus:ring-indigo-500 focus:border-indigo-500 text-sm"
                    placeholder="Opišite detaljno šta vam je potrebno..."
                  />
                </div>

                {submitError && (
                  <div className="bg-red-50 border border-red-200 rounded-lg px-3 py-2 text-sm text-red-700">
                    {submitError}
                  </div>
                )}

                <div className="pt-2 flex gap-3">
                  <button
                    type="button"
                    onClick={resetModal}
                    className="flex-1 py-2.5 border border-gray-300 text-gray-700 rounded-lg font-medium hover:bg-gray-50 text-sm"
                  >
                    Odustani
                  </button>
                  <button
                    type="submit"
                    disabled={submitting || (currentTypeNeedsResource && availableResources.length === 0)}
                    className="flex-1 py-2.5 bg-indigo-600 text-white rounded-lg font-medium hover:bg-indigo-700 text-sm disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    {submitting ? 'Slanje...' : 'Pošalji Zahtjev'}
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