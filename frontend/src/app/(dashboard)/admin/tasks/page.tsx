"use client";

import { useAuth } from '@/context/AuthContext';
import { useEffect, useState, useCallback } from 'react';
import api from '@/lib/api';
import { Request } from '@/types';
import {
  ClipboardList, Clock, CheckCircle, XCircle, Loader2,
  Building2, User, Calendar, X, Download
} from 'lucide-react';
import { exportToExcel } from '@/lib/exportExcel';

const STATUS_OPTIONS = ['Pending', 'InProgress', 'Resolved', 'Rejected'] as const;

const STATUS_CONFIG: Record<string, { label: string; color: string; bg: string; icon: typeof Clock }> = {
  Pending:    { label: 'Na čekanju', color: 'text-amber-700',  bg: 'bg-amber-50 border-amber-200',   icon: Clock },
  InProgress: { label: 'U toku',     color: 'text-blue-700',   bg: 'bg-blue-50 border-blue-200',     icon: Loader2 },
  Resolved:   { label: 'Riješeno',   color: 'text-emerald-700',bg: 'bg-emerald-50 border-emerald-200',icon: CheckCircle },
  Rejected:   { label: 'Odbijeno',   color: 'text-red-700',    bg: 'bg-red-50 border-red-200',       icon: XCircle },
};

const PRIORITY_CONFIG: Record<string, { label: string; color: string; bg: string }> = {
  Low:    { label: 'Nizak',   color: 'text-gray-600',   bg: 'bg-gray-100' },
  Medium: { label: 'Srednji', color: 'text-blue-600',   bg: 'bg-blue-100' },
  High:   { label: 'Visok',   color: 'text-orange-600', bg: 'bg-orange-100' },
  Urgent: { label: 'Hitan',   color: 'text-red-600',    bg: 'bg-red-100' },
};

const TYPE_LABELS: Record<string, string> = {
  Maintenance:          'Održavanje',
  InventoryReplacement: 'Zamjena inventara',
  ResidenceCertificate: 'Potvrda o boravku',
  AccessRequest:        'Zahtjev za pristup',
  Other:                'Ostalo',
  RoomChange:           'Zamjena sobe',
  RoomRepair:           'Popravka u sobi',
  CleaningService:      'Usluga čišćenja',
  KeyReplacement:       'Zamjena ključa/kartice',
  GuestRegistration:    'Registracija gosta',
  InternetSupport:      'Problem s internetom',
  ParkingPermit:        'Dozvola za parking',
  LaundryBooking:       'Rezervacija perionice',
  StorageRequest:       'Zahtjev za ostavu',
  ComplaintReport:      'Prijava problema/žalba',
};

export default function MyTasksPage() {
  useAuth();
  const [tasks, setTasks] = useState<Request[]>([]);
  const [loading, setLoading] = useState(true);
  const [statusModal, setStatusModal] = useState<{ open: boolean; request: Request | null }>({ open: false, request: null });
  const [selectedStatus, setSelectedStatus] = useState('');
  const [actionLoading, setActionLoading] = useState(false);
  const [filterStatus, setFilterStatus] = useState('');

  const fetchTasks = useCallback(async () => {
    try {
      setLoading(true);
      const res = await api.get('/requests/assigned');
      setTasks(res.data);
    } catch (err) {
      console.error('Failed to fetch assigned tasks', err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchTasks(); }, [fetchTasks]);

  const handleStatusUpdate = async () => {
    if (!statusModal.request || !selectedStatus) return;
    setActionLoading(true);
    try {
      await api.put(`/requests/${statusModal.request.id}/status`, { status: selectedStatus });
      setStatusModal({ open: false, request: null });
      setSelectedStatus('');
      fetchTasks();
    } catch (err: any) {
      console.error('Failed to update status', err);
    } finally {
      setActionLoading(false);
    }
  };

  const filtered = filterStatus ? tasks.filter(t => t.status === filterStatus) : tasks;

  const counts = {
    Pending:    tasks.filter(t => t.status === 'Pending').length,
    InProgress: tasks.filter(t => t.status === 'InProgress').length,
    Resolved:   tasks.filter(t => t.status === 'Resolved').length,
    Rejected:   tasks.filter(t => t.status === 'Rejected').length,
  };

  const formatDate = (d: string) => new Date(d).toLocaleDateString('bs-BA', {
    day: '2-digit', month: '2-digit', year: 'numeric',
  });

  const handleExport = () => {
    exportToExcel<Request>('moji_zadaci', [
      { header: 'Naslov',      accessor: t => t.title },
      { header: 'Opis',        accessor: t => t.description },
      { header: 'Tip',         accessor: t => TYPE_LABELS[t.requestType] || t.requestType },
      { header: 'Status',      accessor: t => STATUS_CONFIG[t.status]?.label || t.status },
      { header: 'Prioritet',   accessor: t => PRIORITY_CONFIG[t.priority]?.label || t.priority },
      { header: 'Podnosilac',  accessor: t => t.requestedByName || '' },
      { header: 'Soba',        accessor: t => t.roomNumber || '' },
      { header: 'Resurs',      accessor: t => t.resourceName || '' },
      { header: 'Kreirano',    accessor: t => new Date(t.createdAt) },
    ], filtered);
  };

  return (
    <div className="max-w-5xl mx-auto space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 tracking-tight flex items-center gap-2">
            <ClipboardList className="text-indigo-600" />
            Moji Zadaci
          </h1>
          <p className="text-gray-500 mt-1">
            Zahtjevi dodijeljeni vama — ažurirajte status kada završite
          </p>
        </div>
        <button
          onClick={handleExport}
          disabled={loading || filtered.length === 0}
          className="inline-flex items-center gap-2 px-4 py-2 bg-emerald-600 text-white rounded-lg hover:bg-emerald-700 transition font-medium text-sm disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <Download size={16} /> Izvezi u Excel
        </button>
      </div>

      {/* Summary cards */}
      <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
        {(Object.entries(counts) as [string, number][]).map(([status, count]) => {
          const cfg = STATUS_CONFIG[status];
          const Icon = cfg.icon;
          return (
            <button
              key={status}
              onClick={() => setFilterStatus(filterStatus === status ? '' : status)}
              className={`rounded-xl border p-4 text-left transition-all ${
                filterStatus === status
                  ? `${cfg.bg} ${cfg.color} border-current ring-2 ring-current/30`
                  : 'bg-white border-gray-200 hover:border-gray-300'
              }`}
            >
              <div className={`flex items-center gap-2 text-xs font-medium mb-1 ${filterStatus === status ? cfg.color : 'text-gray-500'}`}>
                <Icon size={14} /> {cfg.label}
              </div>
              <p className={`text-2xl font-bold ${filterStatus === status ? cfg.color : 'text-gray-900'}`}>{count}</p>
            </button>
          );
        })}
      </div>

      {/* Task list */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
        {loading ? (
          <div className="flex items-center justify-center py-20">
            <Loader2 className="animate-spin text-indigo-500" size={28} />
            <span className="ml-3 text-gray-500">Učitavanje zadataka...</span>
          </div>
        ) : filtered.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-20 text-center">
            <ClipboardList size={48} className="text-gray-300 mb-4" />
            <h2 className="text-lg font-medium text-gray-700">
              {filterStatus ? 'Nema zadataka s ovim statusom' : 'Nemate dodijeljenih zadataka'}
            </h2>
            <p className="text-gray-500 mt-1 text-sm">
              {filterStatus ? 'Pokušajte ukloniti filter.' : 'Admin vam još nije dodijelio nijedan zahtjev.'}
            </p>
          </div>
        ) : (
          <ul className="divide-y divide-gray-100">
            {filtered.map(task => {
              const sc = STATUS_CONFIG[task.status] || STATUS_CONFIG.Pending;
              const pc = PRIORITY_CONFIG[task.priority] || PRIORITY_CONFIG.Medium;
              const StatusIcon = sc.icon;
              return (
                <li key={task.id} className="p-5 hover:bg-gray-50/60 transition-colors">
                  <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-4">
                    <div className="flex-1 min-w-0">
                      <div className="flex flex-wrap items-center gap-2 mb-1">
                        <h3 className="font-semibold text-gray-900 truncate">{task.title}</h3>
                        <span className={`inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium border ${sc.bg} ${sc.color}`}>
                          <StatusIcon size={11} />
                          {sc.label}
                        </span>
                        <span className={`inline-flex px-2 py-0.5 rounded-full text-xs font-medium ${pc.bg} ${pc.color}`}>
                          {pc.label}
                        </span>
                      </div>
                      <p className="text-sm text-gray-600 mt-1 line-clamp-2">{task.description}</p>
                      <div className="mt-3 flex flex-wrap gap-x-4 gap-y-1 text-xs text-gray-500">
                        <span className="flex items-center gap-1">
                          <User size={12} />
                          {task.requestedByName}
                        </span>
                        <span className="text-gray-400">
                          {TYPE_LABELS[task.requestType] || task.requestType}
                        </span>
                        {task.roomNumber && (
                          <span className="flex items-center gap-1">
                            <Building2 size={12} /> Soba {task.roomNumber}
                          </span>
                        )}
                        {task.resourceName && (
                          <span className="flex items-center gap-1">
                            <Building2 size={12} /> {task.resourceName}
                          </span>
                        )}
                        <span className="flex items-center gap-1">
                          <Calendar size={12} /> {formatDate(task.createdAt)}
                        </span>
                      </div>
                    </div>
                    <button
                      onClick={() => { setStatusModal({ open: true, request: task }); setSelectedStatus(task.status); }}
                      className="flex-shrink-0 inline-flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium text-emerald-700 bg-emerald-50 hover:bg-emerald-100 rounded-lg transition-colors border border-emerald-200"
                    >
                      <CheckCircle size={13} /> Ažuriraj status
                    </button>
                  </div>
                </li>
              );
            })}
          </ul>
        )}
      </div>

      {/* Status update modal */}
      {statusModal.open && statusModal.request && (
        <div className="fixed inset-0 z-50 flex items-center justify-center">
          <div className="absolute inset-0 bg-gray-900/50 backdrop-blur-sm" onClick={() => setStatusModal({ open: false, request: null })} />
          <div className="relative bg-white rounded-xl shadow-xl border border-gray-200 w-full max-w-sm mx-4 p-6">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-base font-semibold text-gray-900">Ažuriraj status</h3>
              <button onClick={() => setStatusModal({ open: false, request: null })} className="text-gray-400 hover:text-gray-600">
                <X size={20} />
              </button>
            </div>
            <p className="text-sm text-gray-600 mb-1 font-medium">{statusModal.request.title}</p>
            <p className="text-xs text-gray-400 mb-4">Podnosilac: {statusModal.request.requestedByName}</p>
            <label className="block text-sm font-medium text-gray-700 mb-1">Novi status</label>
            <select
              value={selectedStatus}
              onChange={e => setSelectedStatus(e.target.value)}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 mb-4"
            >
              {STATUS_OPTIONS.map(s => (
                <option key={s} value={s}>{STATUS_CONFIG[s].label}</option>
              ))}
            </select>
            <div className="flex justify-end gap-2">
              <button
                onClick={() => setStatusModal({ open: false, request: null })}
                className="px-4 py-2 text-sm text-gray-600 hover:bg-gray-100 rounded-lg transition-colors"
              >
                Otkaži
              </button>
              <button
                onClick={handleStatusUpdate}
                disabled={!selectedStatus || actionLoading}
                className="px-4 py-2 text-sm font-medium text-white bg-emerald-600 hover:bg-emerald-700 rounded-lg transition-colors disabled:opacity-50"
              >
                {actionLoading ? 'Ažuriram...' : 'Ažuriraj'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}