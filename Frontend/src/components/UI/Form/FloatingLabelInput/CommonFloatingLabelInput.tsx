import classes from './CommonFloatingLabelInput.module.css'
import { FC, ReactElement } from 'react';

export interface IInputProps extends React.InputHTMLAttributes<HTMLInputElement>{
    label: string;
}

/// Компонента с общестпользуемой кнопкой.
export const CommonFloatingLabelInput: FC<IInputProps> = ({ label, ...props }): ReactElement => {
    return (
        <div>
            <label htmlFor={props.id} className={classes.commonLabel}>{label}</label>
            <input {...props} className={classes.commonInput} placeholder={label} />
        </div>
    );
}