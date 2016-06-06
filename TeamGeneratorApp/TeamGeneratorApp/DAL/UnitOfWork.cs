using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;
using System.Linq;
using TeamGeneratorApp.DAL.Repositories;


namespace TeamGeneratorApp.DAL
{
    public class UnitOfWork : IDisposable
    {
        private TeamDb6Entities context = new TeamDb6Entities();
        private UserRepository userRepository;
        private RoleRepository roleRepository;
        private CategoryRepository categoryRepository;       
        private EventRepository eventRepository;
        private GroupRepository groupRepository;
        private TeamRepository teamRepository;
        private UserInCategoryRepository userInCategoryRepository;
        private UserInGroupRepository userInGroupRepository;
        private InvitationRepository invitationRepository;
        private UserOnEventRepository userOnEventRepository;
        private PositionRepository positionRepository;


        public UserRepository UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new UserRepository(context);
                }
                return userRepository;
            }
        }

        public RoleRepository RoleRepository
        {
            get
            {
                if (this.roleRepository == null)
                {
                    this.roleRepository = new RoleRepository(context);
                }
                return roleRepository;
            }
        }

        public CategoryRepository CategoryRepository
        {
            get
            {
                if (this.categoryRepository == null)
                {
                    this.categoryRepository = new CategoryRepository(context);
                }
                return categoryRepository;
            }
        }

      
        public EventRepository EventRepository
        {
            get
            {
                if (this.eventRepository == null)
                {
                    this.eventRepository = new EventRepository(context);
                }
                return eventRepository;
            }
        }

        public GroupRepository GroupRepository
        {
            get
            {
                if (this.groupRepository == null)
                {
                    this.groupRepository = new GroupRepository(context);
                }
                return groupRepository;
            }
        }

     

     

        public TeamRepository TeamRepository
        {
            get
            {
                if (this.teamRepository == null)
                {
                    this.teamRepository = new TeamRepository(context);
                }
                return teamRepository;
            }
        }


        public UserInCategoryRepository UserInCategoryRepository
        {
            get
            {
                if (this.userInCategoryRepository == null)
                {
                    this.userInCategoryRepository = new UserInCategoryRepository(context);
                }
                return userInCategoryRepository;
            }
        }

        public UserInGroupRepository UserInGroupRepository
        {
            get
            {
                if (this.userInGroupRepository == null)
                {
                    this.userInGroupRepository = new UserInGroupRepository(context);
                }
                return userInGroupRepository;
            }
        }

        public InvitationRepository InvitationRepository
        {
            get
            {
                if (this.invitationRepository == null)
                {
                    this.invitationRepository = new InvitationRepository(context);
                }
                return invitationRepository;
            }
        }

        public UserOnEventRepository UserOnEventRepository
        {
            get
            {
                if (this.userOnEventRepository == null)
                {
                    this.userOnEventRepository = new UserOnEventRepository(context);
                }
                return userOnEventRepository;
            }
        }

        public PositionRepository PositionRepository
        {
            get
            {
                if (this.positionRepository == null)
                {
                    this.positionRepository = new PositionRepository(context);
                }
                return positionRepository;
            }
        }




        public void SetModified<T>(T item) where T : class
        {
            context.Entry<T>(item).State = EntityState.Modified;
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        public void RollbackChanges()
        {
            context.ChangeTracker.Entries()
                .ToList()
                .ForEach(entry => entry.State = EntityState.Unchanged);
        }


        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
