import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { fetchTodos, deleteTodo, Todo, updateTodo, markComplete } from '../api/todoApi';
import { TodoItem } from './TodoItem';
import { useState } from 'react';

export function TodoList() {
  const [page, setPage] = useState(1);
  const queryClient = useQueryClient();

  const { data, isLoading, isError } = useQuery({
    queryKey: ['todos', { page }],
    queryFn: () => fetchTodos({ pageNumber: page, pageSize: 10 }),
  });

  const deleteMutation = useMutation({
    mutationFn: deleteTodo,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['todos'] });
    },
  });

  const toggleCompleteMutation = useMutation({
    mutationFn: ({ id }: { id: string, isCompleted: boolean }) =>
      markComplete(id),
    onMutate: async ({ id, isCompleted }) => {
      await queryClient.cancelQueries({ queryKey: ['todos'] });

      const previousTodos = queryClient.getQueryData<Todo[]>(['todos']);

      queryClient.setQueryData(['todos'], (old: Todo[] | undefined) => {
        return old?.map(todo =>
          todo.id === id ? { ...todo, isCompleted } : todo
        );
      });

      return { previousTodos };
    },
    onError: (err, variables, context) => {
      if (context?.previousTodos) {
        queryClient.setQueryData(['todos'], context.previousTodos);
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['todos'] });
    }
  });

  const handleToggleComplete = (id: string, isCompleted: boolean) => {
    toggleCompleteMutation.mutate({ id, isCompleted });
  };

  if (isLoading) return <div>Loading...</div>;
  if (isError) return <div>Error fetching todos</div>;

  return (
    <div className="space-y-4">
      {data?.items?.length === 0 ? (
        <div className="text-center text-gray-500">No todos found</div>
      ) : (
        <div className="space-y-4">
          {data?.items?.map((todo: Todo) => (
            <TodoItem
              key={todo.id}
              todo={todo}
              onDelete={deleteMutation.mutate}
              onToggleComplete={handleToggleComplete}
            />
          ))}
        </div>
      )}

      {data.items.length > 0 && <div className="flex items-center justify-between">
        <button
          onClick={() => setPage(p => Math.max(1, p - 1))}
          disabled={page === 1}
          className="rounded-md border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50"
        >
          Previous
        </button>
        <span className="text-sm text-gray-700">
          Page {page} of {data?.totalPages || 1}
        </span>
        <button
          onClick={() => setPage(p => p + 1)}
          disabled={page === data?.totalPages}
          className="rounded-md border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50"
        >
          Next
        </button>
      </div>}
    </div>
  );
}