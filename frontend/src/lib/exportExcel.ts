import * as XLSX from 'xlsx';

export interface ExportColumn<T> {
  header: string;
  accessor: (row: T) => string | number | boolean | null | undefined | Date;
  width?: number;
}

const normalize = (value: unknown): string | number | boolean | Date | null => {
  if (value === null || value === undefined) return null;
  if (value instanceof Date) return isNaN(value.getTime()) ? null : value;
  if (typeof value === 'number' || typeof value === 'boolean') return value;
  return String(value);
};

export function exportToExcel<T>(
  filename: string,
  columns: ExportColumn<T>[],
  rows: T[],
  sheetName: string = 'Podaci'
): void {
  const headers = columns.map(c => c.header);
  const body = rows.map(row => columns.map(c => normalize(c.accessor(row))));

  const aoa: (string | number | boolean | Date | null)[][] = [headers, ...body];

  const ws = XLSX.utils.aoa_to_sheet(aoa, { cellDates: true });

  ws['!cols'] = columns.map((c, idx) => {
    if (c.width) return { wch: c.width };
    const maxLen = Math.max(
      c.header.length,
      ...body.map(r => {
        const v = r[idx];
        if (v === null || v === undefined) return 0;
        if (v instanceof Date) return 19;
        return String(v).length;
      })
    );
    return { wch: Math.min(Math.max(maxLen + 2, 10), 50) };
  });

  const wb = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(wb, ws, sheetName.slice(0, 31));

  const stamp = new Date().toISOString().slice(0, 10);
  const safeName = filename.replace(/[^a-z0-9_\-]/gi, '_');
  XLSX.writeFile(wb, `${safeName}_${stamp}.xlsx`, { bookType: 'xlsx', compression: true });
}
