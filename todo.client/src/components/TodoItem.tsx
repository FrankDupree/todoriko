import { Todo, PriorityLevel } from '../api/todoApi';
import { PencilIcon, TrashIcon, CheckIcon } from '@heroicons/react/20/solid';
import { useNavigate } from 'react-router-dom';

interface TodoItemProps {
  todo: Todo;
  onDelete: (id: string) => void;
  onToggleComplete: (id: string, isCompleted: boolean) => void;
}

const priorityColors = {
  [PriorityLevel.Low]: 'bg-blue-100 text-blue-800',
  [PriorityLevel.Medium]: 'bg-green-100 text-green-800',
  [PriorityLevel.High]: 'bg-yellow-100 text-yellow-800',
  [PriorityLevel.Critical]: 'bg-red-100 text-red-800',
};

export function TodoItem({ todo, onDelete, onToggleComplete }: TodoItemProps) {
  const navigate = useNavigate();
  const priorityLabel = Object.keys(PriorityLevel).find(
    key => PriorityLevel[key as keyof typeof PriorityLevel] === todo.priority
  );

  return (
    <div className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm transition-all hover:shadow-md">
      <div className="flex items-start justify-between">
        <div className="flex-1">
          <div className="flex items-center">
            <button
              onClick={() => onToggleComplete(todo.id, !todo.isCompleted)}
              className={`mr-2 flex h-5 w-5 items-center justify-center rounded-full border cursor-pointer ${
                todo.isCompleted 
                  ? 'border-green-500 bg-green-500 text-white' 
                  : 'border-gray-300 text-transparent'
              }`}
              aria-label={todo.isCompleted ? "Mark as incomplete" : "Mark as complete"}
            >
              <CheckIcon className="h-3 w-3" />
            </button>
            <h3 className={`text-lg font-medium ${todo.isCompleted ? 'line-through text-gray-400' : 'text-gray-900'}`}>
              {todo.title}
            </h3>
          </div>
          {todo.description && (
            <p className="mt-1 text-sm text-gray-600">{todo.description}</p>
          )}
          <div className="mt-2 flex flex-wrap items-center gap-2">
            {todo.tag && (
              <span className="inline-flex items-center rounded-full bg-gray-100 px-2.5 py-0.5 text-xs font-medium text-gray-800">
                {todo.tag}
              </span>
            )}
            <span className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${priorityColors[todo.priority]}`}>
              {priorityLabel}
            </span>
            {todo.dueDate && (
              <span className="text-xs text-gray-500">
                Due: {new Date(todo.dueDate).toLocaleDateString()}
              </span>
            )}
          </div>
        </div>
        <div className="ml-4 flex space-x-2">
          <button
            onClick={() => navigate(`/todos/${todo.id}/edit`)}
            className="cursor-pointer rounded-md p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-500"
          >
            <PencilIcon className="h-5 w-5" />
          </button>
          <button
            onClick={() => onDelete(todo.id)}
            className="cursor-pointer rounded-md p-1 text-gray-400 hover:bg-gray-100 hover:text-red-500"
          >
            <TrashIcon className="h-5 w-5" />
          </button>
        </div>
      </div>
    </div>
  );
}