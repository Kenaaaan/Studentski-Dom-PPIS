"use client";

import { useEffect, useState } from 'react';
import api from '@/lib/api';
import { useAuth } from '@/context/AuthContext';
import { User } from '@/types';
import { Users, Search, Trash2 } from 'lucide-react';
import { toast } from 'react-hot-toast';

export default function AdminUsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const { user: currentUser } = useAuth();

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const response = await api.get('/users');
      setUsers(response.data);
    } catch (error) {
      console.error('Failed to fetch users', error);
      toast.error('Greška pri učitavanju korisnika');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  const handleRoleChange = async (userId: string, newRole: string) => {
    try {
      await api.put(`/users/${userId}/role`, { role: newRole });
      toast.success('Uloga uspješno promijenjena');
      fetchUsers();
    } catch (error) {
      toast.error('Greška pri promjeni uloge');
    }
  };

  const handleStatusChange = async (userId: string, newStatus: string) => {
    try {
      await api.put(`/users/${userId}/status`, { studentStatus: newStatus });
      toast.success('Status uspješno promijenjen');
      fetchUsers();
    } catch (error) {
      toast.error('Greška pri promjeni statusa');
    }
  };

  const handleDeleteUser = async (userId: string) => {
    if (userId === currentUser?.id) {
      toast.error('Ne možete obrisati vlastiti račun');
      return;
    }

    if (!confirm('Jeste li sigurni da želite obrisati ovog korisnika? Ova akcija je nepovratna.')) {
      return;
    }

    try {
      await api.delete(`/users/${userId}`);
      toast.success('Korisnik obrisan');
      setUsers(users.filter(u => u.id !== userId));
    } catch (error) {
      toast.error('Greška pri brisanju korisnika');
    }
  };

  const filteredUsers = users.filter(u => 
    u.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    u.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    u.email.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 tracking-tight flex items-center gap-2">
            <Users className="text-indigo-600" />
            Upravljanje Korisnicima
          </h1>
          <p className="text-gray-500 mt-1">Pregled i upravljanje svim korisnicima sistema</p>
        </div>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
        <div className="p-4 border-b border-gray-200 bg-gray-50 flex items-center gap-4">
          <div className="relative flex-1 max-w-md">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-gray-400">
              <Search size={18} />
            </div>
            <input
              type="text"
              placeholder="Pretraži po imenu, prezimenu ili emailu..."
              className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500 text-sm"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </div>

        {loading ? (
          <div className="p-8 text-center text-gray-500 animate-pulse">Učitavanje korisnika...</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm text-left text-gray-500">
              <thead className="text-xs text-gray-700 uppercase bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-6 py-4">Korisnik</th>
                  <th className="px-6 py-4">Email</th>
                  <th className="px-6 py-4">Uloga</th>
                  <th className="px-6 py-4">Status</th>
                  <th className="px-6 py-4 text-right">Akcije</th>
                </tr>
              </thead>
              <tbody>
                {filteredUsers.length > 0 ? (
                  filteredUsers.map((u) => (
                    <tr key={u.id} className="bg-white border-b border-gray-100 hover:bg-gray-50 transition">
                      <td className="px-6 py-4 font-medium text-gray-900">
                        {u.firstName} {u.lastName}
                      </td>
                      <td className="px-6 py-4">
                        {u.email}
                      </td>
                      <td className="px-6 py-4">
                        {currentUser?.role === 'Admin' ? (
                          <select 
                            className="bg-gray-50 border border-gray-300 text-gray-900 text-xs rounded-lg focus:ring-indigo-500 focus:border-indigo-500 block p-1.5"
                            value={u.role}
                            onChange={(e) => handleRoleChange(u.id, e.target.value)}
                          >
                            <option value="Student">Student</option>
                            <option value="Staff">Osoblje (Staff)</option>
                            <option value="Admin">Administrator</option>
                          </select>
                        ) : (
                          <span className="text-xs font-medium text-gray-700">{u.role}</span>
                        )}
                      </td>
                      <td className="px-6 py-4">
                        {currentUser?.role === 'Admin' ? (
                          <select 
                            className={`border text-xs rounded-lg block p-1.5 ${
                              u.studentStatus === 'Active' ? 'bg-green-50 border-green-300 text-green-800' : 
                              u.studentStatus === 'Suspended' ? 'bg-red-50 border-red-300 text-red-800' :
                              'bg-gray-50 border-gray-300 text-gray-800'
                            }`}
                            value={u.studentStatus}
                            onChange={(e) => handleStatusChange(u.id, e.target.value)}
                          >
                            <option value="Active">Aktivan</option>
                            <option value="Suspended">Suspendovan</option>
                            <option value="MovedOut">Iseljen</option>
                            <option value="Guest">Gost</option>
                          </select>
                        ) : (
                          <span className={`px-2 py-1 text-xs font-semibold rounded-full ${
                            u.studentStatus === 'Active' ? 'bg-green-100 text-green-800' : 
                            u.studentStatus === 'Suspended' ? 'bg-red-100 text-red-800' :
                            'bg-gray-100 text-gray-800'
                          }`}>
                            {u.studentStatus}
                          </span>
                        )}
                      </td>
                      <td className="px-6 py-4 text-right space-x-2">
                        {currentUser?.role === 'Admin' && (
                          <button 
                            onClick={() => handleDeleteUser(u.id)}
                            className="text-red-600 hover:text-red-800 bg-red-50 hover:bg-red-100 p-2 rounded-lg transition"
                            title="Obriši korisnika"
                          >
                            <Trash2 size={16} />
                          </button>
                        )}
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan={5} className="px-6 py-12 text-center text-gray-500">
                      Nema pronađenih korisnika.
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
