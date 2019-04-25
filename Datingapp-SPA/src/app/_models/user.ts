import { Photo } from './Photo';

export interface User {
    id: number;
    username: string;
    gender: string;
    age: number;
    knownAs: string;
    created: Date;
    lastActive: Date;
    introduction?: string;
    lookingFor?: string;
    interests?: string;
    city: string;
    country: string;
    photoUrl: string;
    photos?: Photo[];
}
