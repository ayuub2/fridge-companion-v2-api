﻿using FridgeCompanionV2Api.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.TodoLists.Commands.PurgeTodoLists
{
    public class PurgeTodoListsCommand : IRequest
    {
    }

    public class PurgeTodoListsCommandHandler : IRequestHandler<PurgeTodoListsCommand>
    {
        private readonly IApplicationDbContext _context;

        public PurgeTodoListsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(PurgeTodoListsCommand request, CancellationToken cancellationToken)
        {
            _context.TodoLists.RemoveRange(_context.TodoLists);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
