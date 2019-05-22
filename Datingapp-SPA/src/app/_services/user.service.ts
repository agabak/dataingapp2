import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { UpdateUser } from '../_dtos/updateUser';
import { PagenatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl =  environment.apiUrl;
  constructor(private http: HttpClient) { }

  getUsers(page?, itemPerPage?, userParams?, likesParam?): Observable<PagenatedResult<User[]>> {
      const pagenatedResult: PagenatedResult<User[]> = new PagenatedResult<User[]>();
      let params = new HttpParams();

      if (page !== null && itemPerPage !== null) {
        params = params.append('pageNumber', page);
        params = params.append('pageSize', itemPerPage);
      }

      if (userParams !== null && userParams !== undefined) {
        console.log(userParams);
        params = params.append('minAge', userParams.minAge);
        params = params.append('maxAge', userParams.maxAge);
        params = params.append('gender', userParams.gender);
        params = params.append('orderBy', userParams.orderBy);
      }

      if (likesParam === 'Likers') {
        params = params.append('likers', 'true');
      }

      if (likesParam === 'Likees') {
        params = params.append('likees', 'true');
      }

      return  this.http.get<User[]>(this.baseUrl + 'users', {observe: 'response', params})
               .pipe(
                 map(response => {
                   pagenatedResult.result = response.body;
                   if (response.headers.get('Pagination') !== null) {
                    pagenatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
                   }
                   return pagenatedResult;
                 })
               );
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: UpdateUser) {
   return  this.http.put(this.baseUrl + 'users/' + id, user);
  }

  setMainPhoto(userId: number, id: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
  }

  delete(userId: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }

  sendLike(id: number, recipientId: number) {
    return this.http.post(this.baseUrl + 'users/' + id + '/like/' + recipientId, {});
  }
}
