from flask import Flask, jsonify, render_template, request, flash, render_template, redirect, url_for, session
from DatabaseSetup import Admin, RoomLocation, Room, Service, College, Program, Personnel, RoomReport, ServiceReport, Steps
from peewee import fn
from werkzeug.security import generate_password_hash, check_password_hash
from flask_login import LoginManager, current_user, login_user, login_required, logout_user
from forms import LoginForm, PersonnelForm, RoomsForm, LocationsForm, CollegesForm, ProgramsForm, ServicesForm, StepsForm 
from datetime import datetime, timedelta
from calendar import monthrange

app = Flask(__name__)
app.secret_key = 'M7ND1DEzSxREhKSRav4CMHuoIQUNYFXIQQAaB4DT6wzcvR6jkk'
login_manager = LoginManager()
login_manager.init_app(app)

#Json
@app.route('/room/', methods = ['GET','POST'])
@login_required
def getrooms():
    rooms = []
    for room in Room.select().where(Room.location.is_null(False)).order_by(Room.room_name):
        if room.college is not None:
            rooms.append({
                    'id': room.id,
                    'room_name': room.room_name,
                    'room_desc': room.room_desc,
                    'location': room.location.floor,
                    'xCoord': room.location.xcoord,
                    'yCoord': room.location.ycoord,
                    'zCoord': room.location.zcoord,
                    'college': room.college.college_name,                    
                })
        else:
            rooms.append({
                    'id': room.id,
                    'room_name': room.room_name,
                    'room_desc': room.room_desc,
                    'location': room.location.floor,
                    'xCoord': room.location.xcoord,
                    'yCoord': room.location.ycoord,
                    'zCoord': room.location.zcoord,                    
                })                    
    return jsonify({
            'rooms' : rooms,
        })

@app.route('/personnel/', methods = ['GET','POST'])
@login_required
def getpersonnel():
    personnel = []
    for person in Personnel.select().order_by(Personnel.first_name):
        personnel.append({
                'id': person.id,    
                'first_name': person.first_name,
                'last_name': person.last_name,
                'job_position': person.job_position,
                'assigned_office_id': person.assigned_office.id,
                'assigned_office_name': person.assigned_office.room_name,
            })
    return jsonify({
            'personnel' : personnel,
        })

@app.route('/college/', methods = ['GET','POST'])
@login_required
def getcollege():
    colleges = []

    for college in College.select():
        programs = []
        for program in college.programs:
            programs.append({
                'program_name': program.program_name,
                'program_years': program.program_years,
                'department': program.department,
                })
        colleges.append({
                'college_name': college.college_name,
                'vision': college.vision,
                'mission': college.mission,
                'goal': college.goal,
                'programs': programs,
            })
    return jsonify({
            'colleges': colleges,
        })

@app.route('/service/', methods = ['GET','POST'])
@login_required
def getservice():
    services = []

    for service in Service.select():
        steps = []
        for step in service.steps:
            if step.room is not None:
                steps.append({
                        'step_type': step.step_type,
                        'step_desc': step.step_desc,
                        'xCoord': step.room.location.xcoord,
                        'yCoord': step.room.location.ycoord,
                        'zCoord': step.room.location.zcoord,
                    })
            else:
                steps.append({
                        'step_type': step.step_type,
                        'step_desc': step.step_desc
                    })
        services.append({
                'id': service.id,
                'service_name': service.service_name,
                'service_desc': service.service_desc,
                'steps': steps,
            })
    return jsonify({
            'services': services,
        })

@app.route('/roomreport/<string:operation>/', methods = ['GET','POST'])
@login_required
def getroomreports(operation):
    reports = []
    if operation == "set":
        if request.method == 'POST':
                report_date = request.form['report_date']
                room = request.form['room'];
                reportdata = RoomReport.create(report_date = report_date, room_report = int(room))
                reportdata.save()
                return ('success')
    elif operation == "get":            
        for report in RoomReport.select(fn.Count(RoomReport.id).alias('count'), Room.room_name).join(Room).group_by(RoomReport.room_report).order_by(Room.room_name):
            reports.append({
                    'room_name': report.room_report.room_name,
                    'count': report.count,
                })
        return jsonify({
                'reports' : reports,      
            })

@app.route('/servicereport/<string:operation>/', methods = ['GET','POST'])
@login_required
def getservicereports(operation):
    reports = []
    if operation == "set":
        if request.method == 'POST':
                report_date = request.form['report_date']
                service = request.form['service'];
                reportdata = ServiceReport.create(report_date = report_date, service_report = int(service))
                reportdata.save()
                return ('success')
    elif operation == "get":            
        for report in RoomReport.select(fn.Count(RoomReport.id).alias('count'), Room.room_name).join(Room).group_by(RoomReport.room_report).order_by(Room.room_name):
            reports.append({
                    'room_name': report.room_report.room_name,
                    'count': report.count,
                })
        return jsonify({
                'reports' : reports,      
            })        

#templates 
@app.route('/', methods = ['GET'])
@app.route('/index/', methods = ['GET'])
def index():
    return render_template("index.html")

@app.route('/admin/login/', methods = ['GET','POST'])
def loginadmin():
    if current_user.is_authenticated:
        return redirect(url_for('adminhome'))
    form = LoginForm()
    error = ''
    if form.validate_on_submit():
        try:
            admin = Admin.get(Admin.username == form.username.data)
            if admin:
                if admin.check_password(form.password.data):
                    login_user(admin)
                    return redirect(url_for('adminhome'))
                error = 'The username and password you entered did not match our records'
            error = 'The username and password you entered did not match our records'
            return render_template('adminlogin.html', form = form, error = error)                
        except Exception as e:
            error = 'The username and password you entered did not match our records'
            return render_template('adminlogin.html', form = form, error = error)
    return render_template('adminlogin.html', form = form, error = error)

@login_manager.request_loader
def load_user(request):
    token = request.headers.get('Authorization')
    if token is not None:
        user = Admin.get(Admin.token == token)
        if user:
            return user
    return None

@app.route('/admin/logout/')
@login_required
def logoutadmin(): 
    logout_user()
    return redirect(url_for('loginadmin'))
 
@login_manager.user_loader
def load_user(id):
    return Admin.get(Admin.id == id)

@app.route('/admin/dashboard/', methods=['GET','POST'])
@login_required
def adminhome():    
    room_count = 0
    personnel_count = 0
    views = 0
    freq_rooms = RoomReport.select(fn.Count(RoomReport.id).alias('count'), Room.room_name).join(Room).group_by(RoomReport.room_report).order_by(fn.Count(RoomReport.id).desc()).limit(10)
    freq_services = ServiceReport.select(fn.Count(ServiceReport.id).alias('count'), Service.service_name).join(Service).group_by(ServiceReport.service_report).order_by(fn.Count(ServiceReport.id).desc()).limit(10)
    try:
        room_count = int(Room.select().count())
        personnel_count = int(Personnel.select().count())
        views = int(RoomReport.select().count())
        latest_views = RoomReport.select().order_by(RoomReport.report_date.desc()).limit(10)
        return render_template('admindashboard.html', room_count = room_count, personnel_count = personnel_count, 
                                views = views, latest_views = latest_views, freq_rooms = freq_rooms, freq_services = freq_services)
    except Exception as e:
        flash (e)
    return render_template('admindashboard.html', room_count = room_count, personnel_count = personnel_count,
                         views = views, latest_views = latest_views, freq_rooms = freq_rooms, freq_services = freq_services)

@app.route('/admin/graphdata/<string:operation>', methods=['GET','POST'])
@login_required
def graphdata(operation):
    current_date = datetime.now()
    if operation == "weekly":        
        start_of_week = current_date - timedelta(days = 6)
        labels=[]
        days=[]
        services=[]
        for x in range(0, 7):
            date = start_of_week + timedelta(days=x)
            if date <= current_date:
                count = RoomReport.select(fn.Count(RoomReport.report_date.day).alias('count'), RoomReport.report_date).where(RoomReport.report_date.day == date.day).group_by(RoomReport.report_date.day)              
                if count.exists():
                    for c in count:
                        days.append(c.count)
                else:
                    days.append(0)
                count = ServiceReport.select(fn.Count(ServiceReport.report_date.day).alias('count'), ServiceReport.report_date).where(ServiceReport.report_date.day == date.day).group_by(ServiceReport.report_date.day)
                if count.exists():
                    for c in count:
                        services.append(c.count)
                else:
                    services.append(0)
                labels.append(date.strftime('%b-%d-%Y'))
        return jsonify({
                    "labels": labels,
                    "days": days,
                    "services": services,
            })
    elif operation == "monthly":
        months = []
        labels = []        
        # for x in range(0, 12):
            # current_date.replace(month=x+1)
            # next_month = current_date.replace(month=x)                
            # if current_date.month <= next_month.month:
        count = RoomReport.select(fn.Count(RoomReport.report_date.month).alias('count'), RoomReport.report_date).where(RoomReport.report_date.month <= 
                                    current_date.month, RoomReport.report_date.year >= current_date.year).group_by(RoomReport.report_date.month)                
        # labels.append(all_months.strftime("%B-%Y"))
        if count.exists():
            for c in count:
                months.append(c.count)
                labels.append(c.report_date.strftime("%B-%Y"))
        return jsonify({
                    "labels": labels,
                    "months": months,
            })

@app.route('/admin/rooms/', methods=['GET', 'POST'])
@login_required
def adminrooms():
    room_create_form = RoomsForm(request.form)
    room_edit_form = RoomsForm(request.form)
    location_form = LocationsForm(request.form)
    active = None
    try:
        rooms = Room.select().order_by(Room.is_active)
        locations = RoomLocation.select().order_by(RoomLocation.id)
        return render_template('adminrooms.html', room_create_form = room_create_form, rooms = rooms, room_edit_form = room_edit_form, locations = locations, location_form = location_form, active = active)
    except Exception as e:
        flash (e)   
    return render_template('adminrooms.html', room_create_form = room_create_form, room_edit_form = room_edit_form, locations = locations, location_form = location_form, active = active)             

@app.route('/admin/colleges/<string:operation>', methods=['GET', 'POST'])
@login_required
def admincolleges(operation):
    college_create_form = CollegesForm(request.form)
    college_edit_form = CollegesForm(request.form)
    program_create_form = ProgramsForm(request.form)
    program_edit_form = ProgramsForm(request.form)
    if operation == "view":
        try:
            colleges = College.select().order_by(College.college_name)
            programs = Program.select().order_by(Program.program_name)
            return render_template('admincolleges.html', colleges = colleges, college_create_form = college_create_form, college_edit_form = college_edit_form, 
                                    program_create_form = program_create_form, program_edit_form = program_edit_form, programs = programs)
        except Exception as e:
            flash(e)
            return render_template('admincolleges.html', college_create_form = college_create_form, college_edit_form = college_edit_form, 
                                    program_create_form = program_create_form, program_edit_form = program_edit_form, programs = programs)
    elif operation == "create":
        if (college_create_form.validate_on_submit()):
            college_name = request.form['college_name']
            population = request.form['population']
            vision = request.form['vision']
            mission = request.form['mission']
            goal = request.form['goal']
            college = College.create(college_name=college_name, population=population, vision=vision, mission=mission, goal=goal)
            return jsonify({
                    'id': college.id,
                    'college_name': college.college_name,
                    'population': college.population,
                })
    elif operation == "get":       
        try:
            college = College.get(College.id == request.form['id'])                          
            return jsonify({
                    'id': college.id,
                    'college_name': college.college_name,
                    'population': college.population,
                    'vision': college.vision,
                    'mission': college.mission,
                    'goal': college.goal,
                })
        except Exception as e:
            flash(e)
    elif operation == "edit":
        if (college_edit_form.validate_on_submit()):
            query = College.update(
                        college_name = request.form['college_name'],
                        population = request.form['population'],
                        vision = request.form['vision'],
                        mission = request.form['mission'],
                        goal = request.form['goal']
                    ).where(College.id == request.form['id'])
            query.execute()
            college = College.get(College.id == request.form['id'])
            return jsonify({
                    'college_name': college.college_name,
                    'population': college.population,      
                })
    elif operation == "delete":
        college = College.get(College.id == request.form['id'])
        college_name = college.college_name
        college_id = college.id
        college.delete_instance()      
        return jsonify({
                'college_name': college_name,
                'college_id': college_id,
            })       
    return 'asdd'

@app.route('/admin/programs/<string:operation>', methods=['GET', 'POST'])
@login_required
def adminprograms(operation):
    if operation == "create":
        if ProgramsForm(request.form).validate_on_submit():
            program_name = request.form['program_name']
            program_years = request.form['program_years']
            college = request.form['college']
            program = Program.create(program_name = program_name, program_years = program_years, college = college)
            return jsonify({
                    'id': program.id,
                    'program_name': program.program_name,
                    'program_years': program.program_years,
                    'college': program.college.college_name,
                })
    elif operation == "get":       
        try:
            program = Program.get(Program.id == request.form['id'])                          
            return jsonify({
                    'id': program.id,
                    'program_name': program.program_name,
                    'program_years': program.program_years,
                    'college': program.college.id,
                })
        except Exception as e:
            flash(e)
    elif operation == "edit":
        if (ProgramsForm(request.form).validate_on_submit()):
            query = Program.update(
                        program_name = request.form['program_name'],
                        program_years = request.form['program_years'],
                        college = request.form['college']
                    ).where(Program.id == request.form['id'])
            query.execute()
            program = Program.get(Program.id == request.form['id'])
            return jsonify({
                    'program_name': program.program_name,
                    'program_years': program.program_years,
                    'college': program.college.college_name,      
                }) 
    elif operation == "delete":
        program = Program.get(Program.id == request.form['id'])
        program.delete_instance()
        return 'College has been deleted'     



@app.route('/admin/rooms/locations/<string:operation>', methods=['GET', 'POST'])
@login_required
def locations(operation):
    if operation == "assign":
        if LocationsForm(request.form).validate_on_submit():
            location = request.form['location_name']
            room = request.form['room_name']
            query = Room.update(
                    location = location,
                    is_active = 1
                ).where(Room.id == room)
            query.execute()
            query = RoomLocation.update(
                    occupied = 1
                ).where(RoomLocation.id == location)
            query.execute()
            val = Room.get(Room.id == room)
            return jsonify({
                    'location_id': val.location.id,                   
                    'location_name': val.location.location_name,
                    'room_name': val.room_name,
                    'room_id': val.id,
                    'room_type': val.room_type,
                })
    elif operation =="delete":
        location = RoomLocation.get(RoomLocation.id == request.form['id'])
        print (location.id)
        try:
            room = Room.get(Room.location == location.id)
            query = room.update(
                    location = None,
                    is_active = 0
                    ).where(Room.id == room.id)
            query.execute()
            query = RoomLocation.update(
                    occupied = 0
                ).where(RoomLocation.id == location.id)
            query.execute()
            return jsonify({
                    'location_name': location.location_name,
                    'location_id': location.id,
                    'room_name': room.room_name,
                    'room_id': room.id,
                    'room_type': room.room_type,
                    'empty': 1,
                })
        except Exception as e:
            return jsonify({
                    'location_name': location.location_name,
                    'location_id': location.id,
                    'empty': 0,
                })

@app.route('/admin/rooms/office/<string:operation>/', methods=['GET','POST'])
@login_required
def crudroom(operation):
    if operation == "create":   
        if RoomsForm(request.form).validate_on_submit():
            room_name = request.form['room_name']
            room_desc = request.form['room_desc']
            room_type = request.form['room_type']
            college = request.form['college']
            status = "Inactive"
            if college != "0":
                room =  Room.create(room_name = room_name, room_desc = room_desc, college = college, room_type = room_type)
            else:
                room =  Room.create(room_name = room_name, room_desc = room_desc, room_type = room_type)
            return jsonify({
                    'id': room.id,
                    'room_name': room.room_name,
                    'room_type': room.room_type,
                    'status': status, 
                })
    elif operation == "get":       
        try:
            room = Room.get(Room.id == request.form['id'])
            college = 0
            if room.college is not None:
                college = room.college.id                            
            return jsonify({
                    'id': room.id,
                    'room_name': room.room_name,
                    'room_desc': room.room_desc,
                    'room_type': room.room_type,
                    'college': college,
                })
        except Exception as e:
            flash(e)
    elif operation == "getall":
        rooms = []
        try:
            for room in Room.select().where(Room.is_active == 0):
                rooms.append({
                        'id': room.id,
                        'room_name': room.room_name,
                    })
            return jsonify(rooms)
        except Exception as e:
            flash (e)
    elif operation == "edit":
        location_id = 0
        location_name = None       
        if request.form['college'] == "0":
            query = Room.update(
                    room_name = request.form['room_name'],
                    room_desc = request.form['room_desc'],
                    college = None,
                ).where(Room.id == request.form['id'])
        else:
            query = Room.update(
                    room_name = request.form['room_name'],
                    room_desc = request.form['room_desc'],
                    college = request.form['college']
                ).where(Room.id == request.form['id'])            
        query.execute()
        room = Room.get(Room.id == request.form['id'])
        if room.is_active == 1:
            location_id = room.location.id
            location_name = room.location.location_name
            print(location_id,location_name)
        status = 'Inactive'
        if room.is_active:
            status = 'Active'
        return jsonify({
                'room_name': room.room_name,
                'room_type': room.room_type,
                'status': status,
                'location_id': location_id,
                'location_name': location_name,         
            })                                 
    elif operation == "delete":
        room = Room.get(Room.id == request.form['id'])
        location_id = 0
        location_name = None
        location = 0
        if room.location is not None:
            location = room.id
            location_id = room.location.id
            location_name = room.location.location_name
            query = RoomLocation.update(
                        occupied=0
                    ).where(RoomLocation.id == room.location.id)
            query.execute()            
        room.delete_instance()
        return jsonify({
                'room_id': location,
                'location_id': location_id,
                'location_name': location_name
            })                
    return 'asdasd'

@app.route('/admin/personnel/<string:operation>/', methods=['GET','POST'])
@login_required
def adminpersonnel(operation):
    createform = PersonnelForm(request.form)
    editform = PersonnelForm(request.form)
    if operation == "create" and createform.validate_on_submit():     
        first_name = request.form['first_name']
        last_name = request.form['last_name']
        job_position = request.form['job_position']
        assigned_office = request.form['assigned_office']
        person = Personnel.create(first_name=first_name,last_name=last_name,job_position=job_position,
                                    assigned_office=assigned_office )
        return jsonify({
                    'id' : person.id,
                    'first_name' : person.first_name, 
                    'last_name' : person.last_name,
                    'assigned_office': person.assigned_office.room_name,
                    })
    elif operation == "get":
        person = Personnel.get(Personnel.id == request.form['id'])
        birth_date = person.birth_date.strftime('%Y-%m-%d')
        return jsonify({
                'first_name': person.first_name,
                'last_name': person.last_name,
                'job_position': person.job_position,
                'assigned_office': person.assigned_office.id, 
            })        
    elif operation == "edit" and editform.validate_on_submit():
        query = Personnel.update(
                first_name = request.form['first_name'],
                last_name = request.form['last_name'],
                job_position = request.form['job_position'],
                assigned_office = request.form['assigned_office']
            ).where(Personnel.id == request.form['id'])
        query.execute()
        person = Personnel.get(Personnel.id == request.form['id'])
        return jsonify({
                'first_name': person.first_name,
                'last_name': person.last_name,
                'assigned_office': person.assigned_office.room_name,
            })
    elif operation == "delete":
        person = Personnel.get(Personnel.id == request.form['id'])
        person.delete_instance()
        return 'Person has been deleted'        
    elif operation == "view":       
        try:  
            personnel = Personnel.select()
            return render_template('adminpersonnel.html', personnel = personnel, createform = createform, editform = editform)
        except:
            return render_template('adminpersonnel.html', createform = createform, editform = editform)

@app.route('/admin/services/<string:operation>', methods=['GET','POST'])
@login_required
def adminservices(operation):
    service_create_form = ServicesForm(request.form)
    service_edit_form = ServicesForm(request.form)
    step_create_form = StepsForm(request.form)
    step_edit_form = StepsForm(request.form)
    if operation == "view":
            services = Service.select()
            steps = Steps.select(Steps, fn.Count(Steps.id).alias('count')).group_by(Steps.service).order_by(Steps.service)            
            if services.exists():        
                return render_template('adminservices.html', service_create_form = service_create_form, service_edit_form = service_edit_form, services = services,
                                        step_create_form = step_create_form, step_edit_form = step_edit_form, steps = steps)
            else:
                return render_template('adminservices.html', service_create_form = service_create_form, service_edit_form = service_edit_form,
                                        step_create_form = step_create_form, step_edit_form = step_edit_form, steps = steps)
    elif operation == "createservice":
        if service_create_form.validate_on_submit():
            service_name = request.form['service_name']
            service_desc = request.form['service_desc']
            office = request.form['room']
            print (office)
            service = Service.create(service_name = service_name, service_desc = service_desc, assigned_office = office)
            return jsonify({
                        "id": service.id,
                        "service_name": service.service_name,
                })
    elif operation == "getservice":     
        service = Service.get(Service.id == request.form['id'])
        return jsonify({
                    "service_name": service.service_name,
                    "service_desc": service.service_desc,
                    "room": service.assigned_office.id
            })
    elif operation == "editservice":
        query = Service.update(
                service_name = request.form['service_name'],
                service_desc = request.form['service_desc'],
                assigned_office = request.form['room']
            ).where(Service.id == request.form['id'])
        query.execute()
        service = Service.get(Service.id == request.form['id'])              
        return jsonify({
                    "id": service.id,
                    "service_name": service.service_name,
            })
    elif operation == "deleteservice":
        service = Service.get(Service.id == request.form['id'])
        service_id = service.id
        service_name = service.service_name
        service.delete_instance()
        return jsonify({
                    "id": service_id,
                    "service_name": service_name
            })
    elif operation == "createstep":
        if step_create_form.validate_on_submit():
            step_type = request.form['step_type']
            step_desc = request.form['step_desc']
            service = request.form['service']
            room = None
            is_new = 1
            check_new = Steps.select().where(Steps.service == service)
            if check_new.exists():
                is_new = 0
            if step_type == "Direction":
                room = request.form['room']
            step = Steps.create(step_type = step_type, step_desc = step_desc, room = room,  service = service)
            serv = Service.get(Service.id == service)
            return jsonify({
                        "id": step.service.id,
                        "step_desc": step.step_desc,
                        "service_name": serv.service_name,
                        "is_new": is_new,
                })
    elif operation == "getallsteps":
        steps = []
        num = 0
        for step in Steps.select().where(Steps.service == request.form['id']).order_by(Steps.id):
            num = num + 1
            room = 0
            if step.room is not None:
                room = step.room.id
            steps.append({
                    "service_id": step.service.id,
                    "step_id": step.id,
                    "step_number": "Step {}".format(num),
                    "step_type": step.step_type,
                    "step_desc": step.step_desc,
                    "room": room,
                })
        return jsonify({
                   "steps": steps
            })
    elif operation == "getstep":
        step = Steps.get(Steps.id == request.form['id'])
        room = 0
        if step.room is not None:
            room = step.room.id
        return jsonify({
                    "step_type": step.step_type,
                    "step_desc": step.step_desc,
                    "room": room,
                    "service_id": step.service.id,
            }) 
    elif operation == "editstep":
        room = None
        if request.form['step_type'] == "Direction":
            room = request.form['room']
        query = Steps.update(
                step_type = request.form['step_type'],
                step_desc = request.form['step_desc'],
                room = room
            ).where(Steps.id == request.form['id'])
        query.execute()
        return jsonify("step has been created")
    elif operation == "deletestep":
        service_id = request.form['service']
        print (service_id)
        step = Steps.get(Steps.id == request.form['id'])
        step.delete_instance()
        count = Steps.select(Steps, fn.Count(Steps.id).alias('count')).where(Steps.service == service_id).group_by(Steps.service)
        step_count = 0
        if count.exists():
            for c in count:
                step_count = c.count
        return jsonify({
                    'step_count': step_count,
                    'service_id': service_id,
            })


if __name__ == '__main__':
    app.run(debug=True, port=5000, host = '0.0.0.0')