import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_services/user.service';
import { UpdateUser } from 'src/app/_dtos/updateUser';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
@ViewChild('editForm') editForm: NgForm;
user: User;
updateUse: UpdateUser;
@HostListener('window:beforeunload', ['$event'])
unloadNotification($event: any) {
  if (this.editForm.dirty) {
    $event.returnValue = true;
  }
}
  constructor(private route: ActivatedRoute, private alertify: AlertifyService, private userService: UserService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data.user;
    });
  }

  updateUser() {
    this.updateUse =  {
      introduction: this.user.introduction,
      lookingFor: this.user.lookingFor,
      interests: this.user.interests,
      city: this.user.city,
      country: this.user.country
    };

    this.userService.updateUser(this.user.id, this.updateUse).subscribe(() => {
      this.alertify.success('User updated Successfuly');
      this.editForm.reset(this.user);
    }, error => {
      this.alertify.error(error);
    });
  }

}
