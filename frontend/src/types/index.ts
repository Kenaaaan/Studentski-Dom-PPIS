export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: 'Admin' | 'Staff' | 'Student';
  studentStatus: 'Active' | 'Suspended' | 'MovedOut' | 'Guest';
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface Room {
  id: string;
  roomNumber: string;
  floor: number;
  building: string;
  roomType: 'Single' | 'Double' | 'Triple' | 'Quad';
  capacity: number;
  isAvailable: boolean;
}

export interface Resource {
  id: string;
  name: string;
  description?: string;
  resourceType: 'CommonArea' | 'StudyRoom' | 'Kitchen' | 'Gym' | 'Network' | 'Other';
  location?: string;
  isActive: boolean;
  unavailableUntil?: string;
}

export interface AccessRight {
  id: string;
  userId: string;
  roomId?: string;
  resourceId?: string;
  accessType: 'Room' | 'Building' | 'CommonArea' | 'Network';
  isActive: boolean;
  grantedAt: string;
  expiresAt?: string;
  grantedByUserId: string;
  grantedByName?: string;
  reason?: string;
  resourceName?: string;
  roomNumber?: string;
  userName?: string;
}

export interface Request {
  id: string;
  requestedByUserId: string;
  requestedByName: string;
  roomId?: string;
  roomNumber?: string;
  resourceId?: string;
  resourceName?: string;
  requestType: 'Maintenance' | 'InventoryReplacement' | 'ResidenceCertificate' | 'AccessRequest';
  title: string;
  description: string;
  status: 'Pending' | 'InProgress' | 'Resolved' | 'Rejected';
  priority: 'Low' | 'Medium' | 'High' | 'Urgent';
  assignedToUserId?: string;
  assignedToName?: string;
  createdAt: string;
  updatedAt: string;
  resolvedAt?: string;
  commentCount: number;
}

export interface Comment {
  id: string;
  requestId: string;
  authorUserId: string;
  content: string;
  isInternal: boolean;
  createdAt: string;
  authorName?: string;
}
