export interface Student {
  id: number;
  userName: string;
  email: string;
  photos: Photo[];
}

export interface Photo {
  id: number
  url: string
  isMain: boolean
}
