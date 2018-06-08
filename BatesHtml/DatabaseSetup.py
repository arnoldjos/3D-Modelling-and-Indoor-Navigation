from peewee import *
from werkzeug.security import generate_password_hash, check_password_hash
from flask_login import UserMixin

db = SqliteDatabase('Database.db', pragmas=[
	('foreign_keys', 'on')
])

class BaseModel(Model):
	class Meta:
		database = db

class Admin(UserMixin, BaseModel):
	username = CharField(25)
	password = CharField(80)
	token = CharField(50)

	@classmethod
	def create_user(cls, username, password, token):
		user = Admin(username=username)
		user.set_password(password)
		user.set_token(token)
		user.save()
		return user

	def set_password(self, password):
		self.password = generate_password_hash(password)

	def set_token(self, token):
		self.token = token

	def check_password(self, password):
		return check_password_hash(self.password, password)

class RoomLocation(BaseModel):
	location_name = CharField(5)
	xcoord = FloatField()
	ycoord = FloatField()
	zcoord = FloatField()
	floor = CharField(1)
	occupied = BooleanField(default=False)

class College(BaseModel):
	college_name = CharField(50)
	population = IntegerField()
	vision = TextField()
	mission =  TextField()
	goal = TextField()

class Room(BaseModel):
	room_name = CharField(100)
	room_desc = TextField()
	room_type = CharField(max_length = 25, default='Office')
	is_active = BooleanField(default=False)	
	location = ForeignKeyField(RoomLocation, related_name = 'office', unique=True, null=True)
	college = ForeignKeyField(College, related_name = 'offices', null=True, default=None, on_delete='CASCADE')		

	def __repr__(self):
		return 'Room(id={}, room_name={})'.format(self.id, self.room_name)

class Program(BaseModel):	
	program_name = CharField(100)
	program_years = CharField(1)
	department = CharField(50)
	college = ForeignKeyField(College, related_name = 'programs', unique=False, null=True, on_delete='CASCADE')

class Service(BaseModel):
	service_name = CharField(100)
	service_desc = TextField()
	assigned_office = ForeignKeyField(Room, related_name = 'services', null=True, on_delete='CASCADE')

class Steps(BaseModel):
	step_type = CharField(25)
	step_desc = TextField()
	service = ForeignKeyField(Service, related_name = 'steps', on_delete='CASCADE', null=True)
	room = ForeignKeyField(Room, related_name = 'steps', on_delete='CASCADE', null=True)

class Personnel(BaseModel):
	first_name = CharField(25)
	last_name = CharField(25)
	job_position = CharField(25)
	assigned_office = ForeignKeyField(Room, related_name = 'personnel',null=True, on_delete='CASCADE')

class RoomReport(BaseModel): 
	report_date  = DateTimeField()	
	room_report = ForeignKeyField(Room, related_name = 'reports', on_delete='CASCADE')

	def __repr__(self):
		return 'RoomReport(room_report={}, report_date={})'.format(self.room_report.room_name, self.report_date)

class ServiceReport(BaseModel): 
	report_date  = DateTimeField()	
	service_report = ForeignKeyField(Service, related_name = 'reports', on_delete='CASCADE')

	def __repr__(self):
		return 'RoomReport(room_report={}, report_date={})'.format(self.service_report.service_name, self.report_date)

db.connect()
db.create_tables([Admin, RoomLocation, Room, College, Program, Service, Steps, Personnel, RoomReport, ServiceReport], safe=True)