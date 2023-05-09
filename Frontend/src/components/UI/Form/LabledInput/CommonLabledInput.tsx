import classes from './CommonLabledInput.module.css'
import { FC, ReactElement } from 'react';

export interface IInputProps extends React.InputHTMLAttributes<HTMLInputElement>{
    label: string;
}

/// Компонента с общестпользуемой кнопкой.
export const CommonLabledInput: FC<IInputProps> = ({ label, ...props }): ReactElement => {
    return (
        <div>
            <label className={classes.commonLabel}>{label}</label>
            <input {...props} className={classes.commonInput} />
        </div>
    );
}
