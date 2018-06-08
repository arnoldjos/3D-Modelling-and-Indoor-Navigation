from flask import Flask
from flask_wtf import FlaskForm
from wtforms import StringField, PasswordField, BooleanField, SubmitField, SelectField, TextAreaField, IntegerField
from wtforms.fields.html5 import DateField
from wtforms.validators import InputRequired, Length, DataRequired, Optional
from DatabaseSetup import Room, College, RoomLocation, Service
from werkzeug.utils import secure_filename

class LoginForm(FlaskForm):
	username = StringField('Username', validators=[InputRequired(message='The username and password you entered did not match our records. ')])
	password = PasswordField('Password', validators=[InputRequired(message='The username and password you entered did not match our records. ')])
	submit = SubmitField('Sign In')

class PersonnelForm(FlaskForm):
	first_name = StringField('First Name', validators=[InputRequired(message='empty'),Length(min=2,max=50)])
	last_name = StringField('Last Name', validators=[InputRequired(message='empty'),Length(min=2,max=50)])
	job_position = StringField('Position', validators=[InputRequired(message='empty'),Length(min=2,max=50)])
	assigned_office = SelectField('Assign Office', coerce=int, validators=[InputRequired(message='empty')])
	submit = SubmitField('Save')

	def __init__(self, *args, **kwargs):		
		super().__init__(*args, **kwargs)
		self.assigned_office.choices = [(a.id, a.room_name) for a in Room.select().where(Room.room_type == 'Office', Room.is_active == 1).order_by(Room.room_name)]

class RoomsForm(FlaskForm):
	room_name = StringField('Room Name', [InputRequired(),Length(min=2,max=100)])
	room_desc = TextAreaField('Room Description', [InputRequired()])
	room_type = SelectField('Room Type', choices=[('Office', 'Office'), ('ClassRoom','ClassRoom'),('Cr','Cr'),('Laboratory','Laboratory')])
	college = SelectField('College', coerce=int, validators=[Optional()])
	submit = SubmitField('Save')

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		choices = [(0 ,"None")]
		collegelist = [(a.id, a.college_name) for a in College.select()]
		choices.extend(collegelist)
		self.college.choices = choices 

class LocationsForm(FlaskForm):
	location_floor = SelectField('Floor', choices=[('B', 'Basement'), ('G', 'Ground Floor'), ('2', 'Second Floor')], validators=[Optional()])
	location_name = SelectField('Location', coerce=int, validators=[InputRequired(message='empty')])
	room_name = SelectField('Room Name', coerce=int, validators=[Optional()])
	submit = SubmitField('Save')

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		self.location_name.choices = [(a.id, a.location_name) for a in RoomLocation.select().where(RoomLocation.occupied == 0)]
		self.room_name.choices = [(a.id, a.room_name) for a in Room.select().where(Room.is_active == 0)]	

class CollegesForm(FlaskForm):
	college_name = StringField('College Name', [InputRequired()])
	population = IntegerField('College Population', [InputRequired()])
	vision = TextAreaField('Vision', [InputRequired()])
	mission = TextAreaField('Mission', [InputRequired()])
	goal = TextAreaField('Goal', [InputRequired()])
	submit = SubmitField('Save')

class ProgramsForm(FlaskForm):
	program_name = StringField('Program Name', [InputRequired()])
	program_years = SelectField('Program Years', coerce=int, choices=[(1, '1'), (2, '2'), (3, '3'), (4, '4')])
	department = SelectField('Department')
	college = SelectField('College', coerce=int)
	submit =SubmitField('Save')

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		choices =[
				('Department of Engineering','Department of Engineering'),
				('Department of Computer Studies ','Department of Computer Studies'),
				('Department of Business Administration','Department of Business Administration'),
				('Department of Accountancy','Department of Accountancy'),
				('Department of Elementary Education ','Department of Elementary Education'),
				('Department of Secondary Education','Department of Secondary Education')
			]
		self.department.choices = choices
		self.college.choices = [(a.id, a.college_name) for a in College.select()]

class ServicesForm(FlaskForm):
	service_name = StringField('Service Name', [InputRequired()])
	service_desc = TextAreaField('Description', [InputRequired()])
	room = SelectField('Office', coerce = int)
	submit =SubmitField('Save')

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		self.room.choices = [(a.id, a.room_name) for a in Room.select().where(Room.room_type == "Office", Room.is_active == 1)]

class StepsForm(FlaskForm):
	step_type = SelectField('Type', choices=[('Instruction','Instruction'),('Direction','Direction')])
	step_desc = TextAreaField('Description', [InputRequired()])
	service = SelectField('Service', coerce = int, validators = [InputRequired()])
	room = SelectField('Room Direction', coerce = int, validators = [Optional()])
	submit =SubmitField('Save')

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		self.service.choices = [(a.id, a.service_name) for a in Service.select()]
		self.room.choices = [(a.id, a.room_name) for a in Room.select().where(Room.room_type == "Office", Room.is_active == 1)]
