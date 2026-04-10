"use client";

import { useAuth } from '@/context/AuthContext';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Key, FileText, Clock, AlertCircle } from 'lucide-react';

// A simple dashboard page for students
export default function StudentDashboard() {
  const { user } = useAuth();

  return (
    <div className="space-y-6 max-w-6xl mx-auto">
      <div>
        <h1 className="text-2xl font-bold text-gray-900 tracking-tight">Pregled, {user?.firstName}</h1>
        <p className="text-gray-500 mt-1">Dobrodošli na portal studentskog doma. Vaš status: <span className="font-semibold text-indigo-600">{user?.studentStatus}</span></p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 flex items-start space-x-4 hover:shadow-md transition-shadow">
          <div className="p-3 rounded-lg bg-blue-50 text-blue-600">
            <Key size={24} />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-500">Aktivni pristupi</p>
            <h3 className="text-2xl font-bold text-gray-900 mt-1">3</h3>
          </div>
        </div>

        <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 flex items-start space-x-4 hover:shadow-md transition-shadow">
          <div className="p-3 rounded-lg bg-amber-50 text-amber-600">
            <Clock size={24} />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-500">Zahtjevi u obradi</p>
            <h3 className="text-2xl font-bold text-gray-900 mt-1">1</h3>
          </div>
        </div>

        <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 flex items-start space-x-4 hover:shadow-md transition-shadow">
          <div className="p-3 rounded-lg bg-green-50 text-green-600">
            <FileText size={24} />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-500">Riješeni zahtjevi</p>
            <h3 className="text-2xl font-bold text-gray-900 mt-1">2</h3>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mt-8">
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <div className="px-6 py-5 border-b border-gray-100 flex justify-between items-center">
            <h3 className="font-semibold text-gray-900">Nedavni zahtjevi</h3>
            <button className="text-sm text-indigo-600 font-medium hover:text-indigo-700">Vidi sve</button>
          </div>
          <div className="divide-y divide-gray-100">
            {[1, 2].map((i) => (
              <div key={i} className="p-6 flex items-center justify-between hover:bg-gray-50 transition-colors cursor-pointer">
                <div className="flex items-start gap-4">
                  <div className="mt-1">
                    <AlertCircle className="text-amber-500" size={20} />
                  </div>
                  <div>
                    <h4 className="text-sm font-medium text-gray-900">Popravak radijatora</h4>
                    <p className="text-xs text-gray-500 mt-1">Podneseno: 15. Nov 2025.</p>
                  </div>
                </div>
                <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-amber-100 text-amber-800">
                  U obradi
                </span>
              </div>
            ))}
          </div>
        </div>

        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <div className="px-6 py-5 border-b border-gray-100">
            <h3 className="font-semibold text-gray-900">Brze akcije</h3>
          </div>
          <div className="p-6 grid grid-cols-2 gap-4">
            <button className="flex flex-col items-center justify-center p-4 rounded-lg border-2 border-dashed border-gray-200 hover:border-indigo-500 hover:bg-indigo-50 transition-all text-indigo-600 group">
              <FileText size={28} className="mb-2 text-gray-400 group-hover:text-indigo-600" />
              <span className="text-sm font-medium text-gray-700 group-hover:text-indigo-700">Novi Zahtjev</span>
            </button>
            <button className="flex flex-col items-center justify-center p-4 rounded-lg border-2 border-dashed border-gray-200 hover:border-indigo-500 hover:bg-indigo-50 transition-all text-indigo-600 group">
              <Key size={28} className="mb-2 text-gray-400 group-hover:text-indigo-600" />
              <span className="text-sm font-medium text-gray-700 group-hover:text-indigo-700">Pristup Mreži</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
