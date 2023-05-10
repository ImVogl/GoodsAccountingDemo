import classes from './CommonLabledInput.module.css'
import { FC, ReactElement } from 'react';

export interface IInputProps extends React.InputHTMLAttributes<HTMLInputElement>{
    label: string;
    error?: string;
}

/// Компонента с общестпользуемой кнопкой.
export const CommonLabledInput: FC<IInputProps> = ({ label, error, ...props }): ReactElement => {
    const valid = !(error === null || error === '' || error === undefined);
    return (
        <div>
            <label className={classes.commonLabel}>{label}</label>
            <input  className={valid ? classes.commonInput : classes.error} {...props} />
            <small>{error}</small>
        </div>
    );
}
