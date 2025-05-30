import { TodoList } from '../components/TodoList';
import { TodoForm } from '../components/TodoForm';
import { useState } from 'react';
import { createTodo } from '../api/todoApi';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { PlusIcon } from '@heroicons/react/20/solid';

export function TodosPage() {
  const [isCreating, setIsCreating] = useState(false);
  const queryClient = useQueryClient();

  const createMutation = useMutation({
    mutationFn: createTodo,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['todos'] });
      setIsCreating(false);
    }
  });

  return (
    <div className="mx-auto max-w-4xl space-y-8 p-4 sm:p-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">My Todos</h1>
        <button
          onClick={() => setIsCreating(true)}
          className="inline-flex items-center rounded-md border border-transparent bg-indigo-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
        >
          <PlusIcon className="-ml-1 mr-2 h-5 w-5" />
          Add Todo
        </button>
      </div>

      {isCreating && (
        <div className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm">
          <h2 className="mb-4 text-lg font-medium text-gray-900">Create New Todo</h2>
          <TodoForm
            onSubmit={createMutation.mutate}
            isLoading={createMutation.isPending}
            cancelCreating={() => setIsCreating(false)}
          />
        </div>
      )}

      <TodoList />
    </div>
  );
}