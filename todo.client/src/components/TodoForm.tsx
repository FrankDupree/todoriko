import { useForm } from 'react-hook-form';
import { Todo, PriorityLevel } from '../api/todoApi';
import { CheckIcon, ChevronUpDownIcon, XMarkIcon } from '@heroicons/react/20/solid';

interface TodoFormProps {
  onSubmit: (data: Omit<Todo, 'id' | 'createdAt' | 'isCompleted'>) => void;
  cancelCreating: () => void;
  defaultValues?: Partial<Todo>;
  isLoading?: boolean;
}

const priorityOptions = [
  { value: PriorityLevel.Low, label: 'Low' },
  { value: PriorityLevel.Medium, label: 'Medium' },
  { value: PriorityLevel.High, label: 'High' },
  { value: PriorityLevel.Critical, label: 'Critical' },
];

export function TodoForm({ onSubmit, defaultValues, isLoading, cancelCreating }: TodoFormProps) {
  const { register, handleSubmit, formState: { errors } } = useForm({
    defaultValues: {
      title: '',
      description: '',
      tag: '',
      priority: PriorityLevel.Medium,
      ...defaultValues,
      dueDate: defaultValues?.dueDate ? defaultValues.dueDate.slice(0, 10) : ''
    }
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <div>
        <label htmlFor="title" className="block text-sm font-medium text-gray-700">
          Title *
        </label>
        <input
          id="title"
          type="text"
          {...register('title', { required: 'Title is required' })}
          className={`p-2 mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm ${errors.title ? 'border-red-500' : 'border'}`}
        />
        {errors.title && <p className="mt-1 text-sm text-red-600">{errors.title.message}</p>}
      </div>

      <div>
        <label htmlFor="description" className="block text-sm font-medium text-gray-700">
          Description
        </label>
        <textarea
          id="description"
          rows={3}
          {...register('description')}
          className="p-2 mt-1 block w-full rounded-md border border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
        />
      </div>

      <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
        <div>
          <label htmlFor="dueDate" className="block text-sm font-medium text-gray-700">
            Due Date
          </label>
          <input
            id="dueDate"
            type="date"
            {...register('dueDate', {
              validate: value => {
                if (!value) return true;
                const selectedDate = new Date(value);
                const today = new Date();
                today.setHours(0, 0, 0, 0);
                return selectedDate >= today || "Due date must be today or in the future";
              }
            })}
            className="p-2 mt-1 block w-full rounded-md border border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
          />
          {errors.dueDate && (
            <p className="mt-1 text-sm text-red-600">
              {errors.dueDate.message}
            </p>
          )}
        </div>

        <div>
          <label htmlFor="tag" className="block text-sm font-medium text-gray-700">
            Tag
          </label>
          <input
            id="tag"
            type="text"
            {...register('tag')}
            className="p-2 mt-1 block w-full rounded-md border border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
          />
        </div>
      </div>

      <div>
        <label htmlFor="priority" className="block text-sm font-medium text-gray-700">
          Priority
        </label>
        <select
          id="priority"
          {...register('priority')}
          className="mt-1 block w-full rounded-md border border-gray-300 py-2 pl-3 pr-10 text-base focus:border-indigo-500 focus:outline-none focus:ring-indigo-500 sm:text-sm"
        >
          {priorityOptions.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      </div>

      <div className="flex justify-end gap-4">
        <button
          type="button"
          onClick={cancelCreating}
          className="inline-flex items-center rounded-md border border-transparent bg-red-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 disabled:opacity-50"
        >
          <>
            <XMarkIcon className="-ml-1 mr-2 h-5 w-5" />
            Cancel
          </>
        </button>
        <button
          type="submit"
          disabled={isLoading}
          className="inline-flex items-center rounded-md border border-transparent bg-indigo-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 disabled:opacity-50"
        >
          {isLoading ? (
            'Saving...'
          ) : (
            <>
              <CheckIcon className="-ml-1 mr-2 h-5 w-5" />
              Save
            </>
          )}
        </button>
      </div>
    </form>
  );
}