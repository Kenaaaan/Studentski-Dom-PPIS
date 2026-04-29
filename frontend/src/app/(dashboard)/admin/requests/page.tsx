"use client";

import { useAuth } from '@/context/AuthContext';
import { useEffect, useState, useCallback } from 'react';
import api from '@/lib/api';
import { Request, User, Room } from '@/types';
import {
  FileText, Search, Filter, UserPlus, ChevronDown, X,
  Clock, CheckCircle, XCircle, AlertTriangle, Loader2,
  MessageSquare, Building2
} from 'lucide-react';

const STATUS_OPTIONS = ['Pending', 'InProgress', 'Resolved', 'Rejected'] as const;
const TYPE_OPTIONS = ['Maintenance', 'InventoryReplacement', 'ResidenceCertificate'] as const;
const PRIORITY_OPTIONS = ['Low', 'Medium', 'High', 'Urgent'] as const;

const STATUS_CONFIG: Record<string, { label: string; color: string; bg: string; icon: typeof Clock }> = {
  Pending: { label: 'Na čekanju', color: 'text-amber-700', bg: 'bg-amber-50 border-amber-200', icon: Clock },
  InProgress: { label: 'U toku', color: 'text-blue-700', bg: 'bg-blue-50 border-blue-200', icon: Loader2 },
  Resolved: { label: 'Riješeno', color: 'text-emerald-700', bg: 'bg-emerald-50 border-emerald-200', icon: CheckCircle },
  Rejected: { label: 'Odbijeno', color: 'text-red-700', bg: 'bg-red-50 border-red-200', icon: XCircle },
};

const PRIORITY_CONFIG: Record<string, { label: string; color: string; bg: string }> = {
  Low: { label: 'Nizak', color: 'text-gray-600', bg: 'bg-gray-100' },
  Medium: { label: 'Srednji', color: 'text-blue-600', bg: 'bg-blue-100' },
  High: { label: 'Visok', color: 'text-orange-600', bg: 'bg-orange-100' },
  Urgent: { label: 'Hitan', color: 'text-red-600', bg: 'bg-red-100' },
};

const TYPE_LABELS: Record<string, string> = {
  Maintenance: 'Održavanje',
  InventoryReplacement: 'Zamjena inventara',
  ResidenceCertificate: 'Potvrda o stanovanju',
};

interface Filters {
  status: string;
  type: string;
  priority: string;
  assignedToUserId: string;
  requestedByUserId: string;
  roomId: string;
  search: string;
}

const emptyFilters: Filters = {
  status: '', type: '', priority: '',
  assignedToUserId: '', requestedByUserId: '', roomId: '', search: '',
};

export default function AdminRequestsPage() {
  const { user } = useAuth();
  const [requests, setRequests] = useState<Request[]>([]);
  const [staffUsers, setStaffUsers] = useState<User[]>([]);
  const [allUsers, setAllUsers] = useState<User[]>([]);
  const [rooms, setRooms] = useState<Room[]>([]);
  const [loading, setLoading] = useState(true);
  const [filters, setFilters] = useState<Filters>(emptyFilters);
  const [showFilters, setShowFilters] = useState(false);

  // Modal state
  const [assignModal, setAssignModal] = useState<{ open: boolean; request: Request | null }>({ open: false, request: null });
  const [statusModal, setStatusModal] = useState<{ open: boolean; request: Request | null }>({ open: false, request: null });
  const [selectedStaffId, setSelectedStaffId] = useState('');
  const [selectedStatus, setSelectedStatus] = useState('');
  const [actionLoading, setActionLoading] = useState(false);

  const fetchRequests = useCallback(async () => {
    try {
      setLoading(true);
      const params: Record<string, string> = {};
      if (filters.status) params.status = filters.status;
      if (filters.type) params.type = filters.type;
      if (filters.priority) params.priority = filters.priority;
      if (filters.assignedToUserId) params.assignedToUserId = filters.assignedToUserId;
      if (filters.requestedByUserId) params.requestedByUserId = filters.requestedByUserId;
      if (filters.roomId) params.roomId = filters.roomId;
      if (filters.search) params.search = filters.search;
      const res = await api.get('/requests', { params });
      setRequests(res.data);
    } catch (err) {
      console.error('Failed to fetch requests', err);
    } finally {
      setLoading(false);
    }
  }, [filters]);

  useEffect(() => { fetchRequests(); }, [fetchRequests]);

  useEffect(() => {
    const loadMeta = async () => {
      try {
        const [staffRes, usersRes, roomsRes] = await Promise.all([
          api.get('/users/staff'),
          api.get('/users'),
          api.get('/rooms'),
        ]);
        setStaffUsers(staffRes.data);
        setAllUsers(usersRes.data);
        setRooms(roomsRes.data);
      } catch (err) {
        console.error('Failed to load metadata', err);
      }
    };
    loadMeta();
  }, []);

  const handleAssign = async () => {
    if (!assignModal.request || !selectedStaffId) return;
    setActionLoading(true);
    try {
      await api.put(`/requests/${assignModal.request.id}/assign`, { assignedToUserId: selectedStaffId });
      setAssignModal({ open: false, request: null });
      setSelectedStaffId('');
      fetchRequests();
    } catch (err) {
      console.error('Failed to assign', err);
    } finally {
      setActionLoading(false);
    }
  };

  const handleStatusUpdate = async () => {
    if (!statusModal.request || !selectedStatus) return;
    setActionLoading(true);
    try {
      await api.put(`/requests/${statusModal.request.id}/status`, { status: selectedStatus });
      setStatusModal({ open: false, request: null });
      setSelectedStatus('');
      fetchRequests();
    } catch (err) {
      console.error('Failed to update status', err);
    } finally {
      setActionLoading(false);
    }
  };

  const activeFilterCount = Object.values(filters).filter(v => v !== '').length;

  const resetFilters = () => setFilters(emptyFilters);

  const formatDate = (d: string) => new Date(d).toLocaleDateString('bs-BA', {
    day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit'
  });

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 tracking-tight flex items-center gap-2">
            <FileText className="text-indigo-600" />
            Upravljanje Zahtjevima
          </h1>
          <p className="text-gray-500 mt-1">
            Pregled svih zahtjeva studenata — filtriranje, dodjela i ažuriranje statusa
          </p>
        </div>
        <div className="flex items-center gap-2">
          <span className="text-sm text-gray-500">{requests.length} zahtjev(a)</span>
        </div>
      </div>

      {/* Search & Filter Bar */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-4">
        <div className="flex flex-col sm:flex-row gap-3">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={18} />
            <input
              type="text"
              placeholder="Pretraži po naslovu ili opisu..."
              value={filters.search}
              onChange={e => setFilters(f => ({ ...f, search: e.target.value }))}
              className="w-full pl-10 pr-4 py-2.5 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
            />
          </div>
          <button
            onClick={() => setShowFilters(!showFilters)}
            className={`inline-flex items-center gap-2 px-4 py-2.5 rounded-lg text-sm font-medium border transition-colors ${
              showFilters || activeFilterCount > 0
                ? 'bg-indigo-50 text-indigo-700 border-indigo-200'
                : 'bg-white text-gray-700 border-gray-200 hover:bg-gray-50'
            }`}
          >
            <Filter size={16} />
            Filteri
            {activeFilterCount > 0 && (
              <span className="bg-indigo-600 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                {activeFilterCount}
              </span>
            )}
            <ChevronDown size={14} className={`transition-transform ${showFilters ? 'rotate-180' : ''}`} />
          </button>
          {activeFilterCount > 0 && (
            <button onClick={resetFilters} className="inline-flex items-center gap-1 px-3 py-2.5 text-sm text-red-600 hover:bg-red-50 rounded-lg transition-colors">
              <X size={14} /> Resetuj
            </button>
          )}
        </div>

        {/* Expanded Filters */}
        {showFilters && (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3 mt-4 pt-4 border-t border-gray-100">
            <FilterSelect label="Status" value={filters.status} onChange={v => setFilters(f => ({ ...f, status: v }))}>
              {STATUS_OPTIONS.map(s => <option key={s} value={s}>{STATUS_CONFIG[s].label}</option>)}
            </FilterSelect>
            <FilterSelect label="Tip zahtjeva" value={filters.type} onChange={v => setFilters(f => ({ ...f, type: v }))}>
              {TYPE_OPTIONS.map(t => <option key={t} value={t}>{TYPE_LABELS[t]}</option>)}
            </FilterSelect>
            <FilterSelect label="Prioritet" value={filters.priority} onChange={v => setFilters(f => ({ ...f, priority: v }))}>
              {PRIORITY_OPTIONS.map(p => <option key={p} value={p}>{PRIORITY_CONFIG[p].label}</option>)}
            </FilterSelect>
            <FilterSelect label="Dodijeljeni korisnik" value={filters.assignedToUserId} onChange={v => setFilters(f => ({ ...f, assignedToUserId: v }))}>
              {staffUsers.map(u => <option key={u.id} value={u.id}>{u.firstName} {u.lastName}</option>)}
            </FilterSelect>
            <FilterSelect label="Podnosilac zahtjeva" value={filters.requestedByUserId} onChange={v => setFilters(f => ({ ...f, requestedByUserId: v }))}>
              {allUsers.map(u => <option key={u.id} value={u.id}>{u.firstName} {u.lastName}</option>)}
            </FilterSelect>
            <FilterSelect label="Soba" value={filters.roomId} onChange={v => setFilters(f => ({ ...f, roomId: v }))}>
              {rooms.map(r => <option key={r.id} value={r.id}>{r.roomNumber}</option>)}
            </FilterSelect>
          </div>
        )}
      </div>

      {/* Requests Table */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
        {loading ? (
          <div className="flex items-center justify-center py-20">
            <Loader2 className="animate-spin text-indigo-500" size={32} />
            <span className="ml-3 text-gray-500">Učitavanje zahtjeva...</span>
          </div>
        ) : requests.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-20 text-center">
            <FileText size={48} className="text-gray-300 mb-4" />
            <h2 className="text-xl font-medium text-gray-700">Nema zahtjeva</h2>
            <p className="text-gray-500 mt-2">Nema zahtjeva koji odgovaraju trenutnim filterima.</p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="bg-gray-50 border-b border-gray-200">
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Naslov</th>
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Tip</th>
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Status</th>
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Prioritet</th>
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Podnosilac</th>
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Dodijeljeno</th>
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Soba</th>
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Datum</th>
                  <th className="text-right px-4 py-3 font-semibold text-gray-600">Akcije</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {requests.map(req => {
                  const sc = STATUS_CONFIG[req.status] || STATUS_CONFIG.Pending;
                  const pc = PRIORITY_CONFIG[req.priority] || PRIORITY_CONFIG.Medium;
                  const StatusIcon = sc.icon;
                  return (
                    <tr key={req.id} className="hover:bg-gray-50/50 transition-colors">
                      <td className="px-4 py-3">
                        <div className="font-medium text-gray-900 max-w-[200px] truncate" title={req.title}>{req.title}</div>
                        <div className="text-xs text-gray-400 mt-0.5 max-w-[200px] truncate" title={req.description}>{req.description}</div>
                      </td>
                      <td className="px-4 py-3">
                        <span className="text-gray-600 text-xs">{TYPE_LABELS[req.requestType] || req.requestType}</span>
                      </td>
                      <td className="px-4 py-3">
                        <span className={`inline-flex items-center gap-1 px-2 py-1 rounded-full text-xs font-medium border ${sc.bg} ${sc.color}`}>
                          <StatusIcon size={12} />
                          {sc.label}
                        </span>
                      </td>
                      <td className="px-4 py-3">
                        <span className={`inline-flex px-2 py-0.5 rounded-full text-xs font-medium ${pc.bg} ${pc.color}`}>
                          {pc.label}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-gray-600 text-xs">{req.requestedByName || '—'}</td>
                      <td className="px-4 py-3 text-gray-600 text-xs">{req.assignedToName || <span className="text-gray-400 italic">Nedodijeljeno</span>}</td>
                      <td className="px-4 py-3 text-gray-600 text-xs">
                        {req.roomNumber ? (
                          <span className="inline-flex items-center gap-1"><Building2 size={12} />{req.roomNumber}</span>
                        ) : '—'}
                      </td>
                      <td className="px-4 py-3 text-gray-400 text-xs whitespace-nowrap">{formatDate(req.createdAt)}</td>
                      <td className="px-4 py-3 text-right">
                        <div className="flex items-center justify-end gap-1">
                          <button
                            onClick={() => { setAssignModal({ open: true, request: req }); setSelectedStaffId(req.assignedToUserId || ''); }}
                            className="inline-flex items-center gap-1 px-2.5 py-1.5 text-xs font-medium text-indigo-700 bg-indigo-50 hover:bg-indigo-100 rounded-md transition-colors"
                            title="Dodijeli"
                          >
                            <UserPlus size={13} /> Dodijeli
                          </button>
                          <button
                            onClick={() => { setStatusModal({ open: true, request: req }); setSelectedStatus(req.status); }}
                            className="inline-flex items-center gap-1 px-2.5 py-1.5 text-xs font-medium text-emerald-700 bg-emerald-50 hover:bg-emerald-100 rounded-md transition-colors"
                            title="Status"
                          >
                            <CheckCircle size={13} /> Status
                          </button>
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Assign Modal */}
      {assignModal.open && assignModal.request && (
        <Modal title="Dodijeli zahtjev" onClose={() => { setAssignModal({ open: false, request: null }); setSelectedStaffId(''); }}>
          <p className="text-sm text-gray-600 mb-1">Zahtjev: <strong>{assignModal.request.title}</strong></p>
          <p className="text-xs text-gray-400 mb-4">Podnosilac: {assignModal.request.requestedByName}</p>
          <label className="block text-sm font-medium text-gray-700 mb-1">Dodijeli korisniku (Staff/Admin)</label>
          <select
            value={selectedStaffId}
            onChange={e => setSelectedStaffId(e.target.value)}
            className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 mb-4"
          >
            <option value="">— Odaberi —</option>
            {staffUsers.map(u => <option key={u.id} value={u.id}>{u.firstName} {u.lastName} ({u.role})</option>)}
          </select>
          <div className="flex justify-end gap-2">
            <button onClick={() => setAssignModal({ open: false, request: null })} className="px-4 py-2 text-sm text-gray-600 hover:bg-gray-100 rounded-lg transition-colors">Otkaži</button>
            <button onClick={handleAssign} disabled={!selectedStaffId || actionLoading} className="px-4 py-2 text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 rounded-lg transition-colors disabled:opacity-50">
              {actionLoading ? 'Dodjeljujem...' : 'Dodijeli'}
            </button>
          </div>
        </Modal>
      )}

      {/* Status Modal */}
      {statusModal.open && statusModal.request && (
        <Modal title="Ažuriraj status" onClose={() => { setStatusModal({ open: false, request: null }); setSelectedStatus(''); }}>
          <p className="text-sm text-gray-600 mb-1">Zahtjev: <strong>{statusModal.request.title}</strong></p>
          <p className="text-xs text-gray-400 mb-4">Trenutni status: {STATUS_CONFIG[statusModal.request.status]?.label}</p>
          <label className="block text-sm font-medium text-gray-700 mb-1">Novi status</label>
          <select
            value={selectedStatus}
            onChange={e => setSelectedStatus(e.target.value)}
            className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 mb-4"
          >
            {STATUS_OPTIONS.map(s => <option key={s} value={s}>{STATUS_CONFIG[s].label}</option>)}
          </select>
          <div className="flex justify-end gap-2">
            <button onClick={() => setStatusModal({ open: false, request: null })} className="px-4 py-2 text-sm text-gray-600 hover:bg-gray-100 rounded-lg transition-colors">Otkaži</button>
            <button onClick={handleStatusUpdate} disabled={!selectedStatus || actionLoading} className="px-4 py-2 text-sm font-medium text-white bg-emerald-600 hover:bg-emerald-700 rounded-lg transition-colors disabled:opacity-50">
              {actionLoading ? 'Ažuriram...' : 'Ažuriraj'}
            </button>
          </div>
        </Modal>
      )}
    </div>
  );
}

/* ── Reusable sub-components ── */

function FilterSelect({ label, value, onChange, children }: {
  label: string; value: string; onChange: (v: string) => void; children: React.ReactNode;
}) {
  return (
    <div>
      <label className="block text-xs font-medium text-gray-500 mb-1">{label}</label>
      <select
        value={value}
        onChange={e => onChange(e.target.value)}
        className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent bg-white"
      >
        <option value="">Svi</option>
        {children}
      </select>
    </div>
  );
}

function Modal({ title, onClose, children }: { title: string; onClose: () => void; children: React.ReactNode }) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="absolute inset-0 bg-gray-900/50 backdrop-blur-sm" onClick={onClose} />
      <div className="relative bg-white rounded-xl shadow-xl border border-gray-200 w-full max-w-md mx-4 p-6 animate-in">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-semibold text-gray-900">{title}</h3>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600 transition-colors"><X size={20} /></button>
        </div>
        {children}
      </div>
    </div>
  );
}
